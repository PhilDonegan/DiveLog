using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using DiveLog.DTO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Data.SQLite;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using DiveLog.Parser.Extension;
using DiveLog.Parser.Progress;

namespace DiveLog.Parsers
{
    public class Shearwater : IParser
    {
        private IConfiguration _builder;
		private int _totalDives;

		public delegate void ParserProgressEventArgs(string id, ParserProgess parserProgess);
		public event ParserProgressEventArgs DiveParsed;

        public Shearwater(IConfiguration builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public async Task<List<LogEntryDTO>> ProcessDivesAsync(string id, string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var sqliteConnection = CreateSqliteConnection(path);
            var dives = ExtractDives(id, sqliteConnection);

			return dives;
        }

        private List<LogEntryDTO> ExtractDives(string id, SQLiteConnection sqliteConnection)
        {
            List<LogEntryDTO> dives = new List<LogEntryDTO>();
            try
            {
                sqliteConnection.Open();

				if (DiveParsed != null)
				{
					string countSql = "SELECT COUNT(id) FROM dive_logs";
					var countCommand = new SQLiteCommand(countSql, sqliteConnection);
					_totalDives = int.Parse(countCommand.ExecuteScalar().ToString());

					DiveParsed(id, new ParserProgess(0, _totalDives));
				}

                var sql = "SELECT id, datetime(startTimeStamp,'unixepoch') as stringStartDate, maxTime, maxDepth FROM dive_logs";
                using (var command = new SQLiteCommand(sql, sqliteConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        { 
                            var dive = new LogEntryDTO();
                            dive.ExternalId = reader["id"].ToString();

                            var date = reader["stringStartDate"];

                            dive.DiveDate = DateTime.Parse(date.ToString());
                            dive.MaxDepth = Convert.ToDecimal(reader["maxDepth"]);
                            dive.DiveLength = TimeSpan.FromMinutes(Convert.ToInt32(reader["maxTime"]));
                            dive.DiveType = DTO.Types.DiveType.CCR;
                            dive.Outcome = DTO.Types.DiveOutcome.Unknown;

                            dive.DataPoints = new List<DataPointDTO>();
                            dives.Add(dive);
						} 
                    }
                }

                string sqllog = "SELECT id, diveLogId, currentTime, currentDepth, fractionO2, fractionHe, waterTemp, averagePPO2, CNSPercent FROM dive_log_records WHERE diveLogId =";
                foreach(var dive in dives)
                {
                    using (var command = new SQLiteCommand(sqllog + dive.ExternalId, sqliteConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (dive.FractionHe == 0 && dive.FractionO2 == 0)
                                {
                                    dive.FractionO2 = Convert.ToDecimal(reader["fractionO2"]);
                                    dive.FractionHe = Convert.ToDecimal(reader["fractionHe"]);
                                }

                                var dataPoint = new DataPointDTO();
                                dataPoint.AveragePPO2 = Convert.ToDecimal(reader["averagePPO2"]);
                                dataPoint.Depth = Convert.ToDecimal(reader["currentDepth"]);
                                dataPoint.Time = Convert.ToInt32(reader["currentTime"]);
                                dataPoint.WaterTemp = Convert.ToInt16(reader["waterTemp"]);
                                dataPoint.CNS = Convert.ToInt16(reader["CNSPercent"]);

                                dive.DataPoints.Add(dataPoint);
							}
                        }
                    }

					DiveParsed?.Invoke(id, new ParserProgess(dives.IndexOf(dive) + 1, _totalDives));
				}
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            finally
            {
                sqliteConnection.Close();
            }

            return dives;
        }

        private SQLiteConnection CreateSqliteConnection(string path)
        {
            return new SQLiteConnection($"Data Source={path};datetimeformat={CultureInfo.CurrentCulture}");
        }
    }
}
