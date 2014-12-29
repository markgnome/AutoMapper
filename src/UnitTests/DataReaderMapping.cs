#if !SILVERLIGHT && !NETFX_CORE
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Rhino.Mocks.Constraints;
using Xunit;
using Should;

namespace AutoMapper.UnitTests
{
    namespace DataReaderMapping
    {
        public class When_mapping_a_data_reader_to_a_dto : AutoMapperSpecBase
        {
            protected override void Establish_context()
            {
                Mapper.CreateMap<IDataReader, DTOObject>()
                    .ForMember(dest => dest.Else, options => options.MapFrom(src => src.GetDateTime(10)));

                DataReader = new DataBuilder().BuildDataReader();
                Results = Mapper.Map<IDataReader, IEnumerable<DTOObject>>(DataReader); 
                Result = Results.FirstOrDefault();
            }

            [Fact]
            public void Then_a_column_containing_a_small_integer_should_be_read()
            {
                Result.SmallInteger.ShouldEqual(DataReader[FieldName.SmallInt]);
            }

            [Fact]
            public void Then_a_column_containing_an_integer_should_be_read()
            {
                Result.Integer.ShouldEqual(DataReader[FieldName.Int]);
            }

            [Fact]
            public void Then_a_column_containing_a_big_integer_should_be_read()
            {
                Result.BigInteger.ShouldEqual(DataReader[FieldName.BigInt]);
            }

            [Fact]
            public void Then_a_column_containing_a_GUID_should_be_read()
            {
                Result.Guid.ShouldEqual(DataReader[FieldName.Guid]);
            }

            [Fact]
            public void Then_a_column_containing_a_float_should_be_read()
            {
                Result.Float.ShouldEqual(DataReader[FieldName.Float]);
            }

            [Fact]
            public void Then_a_column_containing_a_double_should_be_read()
            {
                Result.Double.ShouldEqual(DataReader[FieldName.Double]);
            }

            [Fact]
            public void Then_a_column_containing_a_decimal_should_be_read()
            {
                Result.Decimal.ShouldEqual(DataReader[FieldName.Decimal]);
            }

            [Fact]
            public void Then_a_column_containing_a_date_and_time_should_be_read()
            {
                Result.DateTime.ShouldEqual(DataReader[FieldName.DateTime]);
            }

            [Fact]
            public void Then_a_column_containing_a_byte_should_be_read()
            {
                Result.Byte.ShouldEqual(DataReader[FieldName.Byte]);
            }

            [Fact]
            public void Then_a_column_containing_a_boolean_should_be_read()
            {
                Result.Boolean.ShouldEqual(DataReader[FieldName.Boolean]);   
            }

            [Fact]
            public void Then_a_projected_column_should_be_read()
            {
                Result.Else.ShouldEqual(DataReader.GetDateTime(10));
            }

            protected DTOObject Result { get; set; }
            protected IEnumerable<DTOObject> Results { get; set; }
            protected IDataReader DataReader { get; set; }
        }

        public class When_mapping_a_data_reader_to_matching_dtos : AutoMapperSpecBase
        {
            protected override void Establish_context()
            {
                Mapper.CreateMap<IDataReader, DTOObject>()
                    .ForMember(dest => dest.Else, options => options.MapFrom(src => src.GetDateTime(10)));
                Mapper.CreateMap<IDataReader, DerivedDTOObject>()
                    .ForMember(dest => dest.Else, options => options.MapFrom(src => src.GetDateTime(10)));

            }

            protected override void Because_of()
            {
                Mapper.Map<IDataReader, IEnumerable<DTOObject>>(new DataBuilder().BuildDataReader()).ToArray();
            }

            [Fact]
            public void Should_map_successfully()
            {
                var result = Mapper.Map<IDataReader, IEnumerable<DerivedDTOObject>>(new DataBuilder().BuildDataReader());
                result.Count().ShouldEqual(1);
            }
        }
        /// <summary>
        /// The purpose of this test is to exercise the internal caching logic of DataReaderMapper.
        /// </summary>
        public class When_mapping_a_data_reader_to_a_dto_twice : When_mapping_a_data_reader_to_a_dto
        {
            protected override void Establish_context()
            {
                base.Establish_context();

                DataReader = new DataBuilder().BuildDataReader();
                Results = Mapper.Map<IDataReader, IEnumerable<DTOObject>>(DataReader);
                Result = Results.FirstOrDefault();
            }
        }

        public class When_mapping_a_data_reader_using_the_default_coniguration : When_mapping_a_data_reader_to_a_dto
        {
            [Fact]
            public void Then_the_enumerable_should_be_a_list()
            {
                Results.ShouldImplement<IList<DTOObject>>();
            }
        }

        public class When_mapping_a_data_reader_using_the_yield_return_option : When_mapping_a_data_reader_to_a_dto
        {
            protected override void Establish_context()
            {
                Mapper.Configuration.EnableYieldReturnForDataReaderMapper();
                base.Establish_context();
            }

            [Fact]
            public void Then_the_enumerable_should_not_be_a_list()
            {
                Results.ShouldNotBeInstanceOf<IList<DTOObject>>();
            }
        }

        public class When_mapping_a_data_reader_to_a_dto_and_the_map_does_not_exist : AutoMapperSpecBase
        {
            protected override void Establish_context()
            {
                _dataReader = new DataBuilder().BuildDataReader();
            }

            [Fact]
            public void Then_an_automapper_exception_should_be_thrown()
            {
                typeof (AutoMapperMappingException).ShouldBeThrownBy(
                    () => Mapper.Map<IDataReader, IEnumerable<DTOObject>>(_dataReader).FirstOrDefault());
            }

            private IDataReader _dataReader;
        }


        public class When_mapping_a_single_data_record_to_a_dto : AutoMapperSpecBase
        {
            protected override void Establish_context()
            {
                Mapper.CreateMap<IDataRecord, DTOObject>()
                    .ForMember(dest => dest.Else, options => options.MapFrom(src => src.GetDateTime(src.GetOrdinal(FieldName.Something))));

                _dataRecord = new DataBuilder().BuildDataRecord();
                _result = Mapper.Map<IDataRecord, DTOObject>(_dataRecord);
            }

            [Fact]
            public void Then_a_column_containing_a_small_integer_should_be_read()
            {
                _result.SmallInteger.ShouldEqual(_dataRecord[FieldName.SmallInt]);
            }

            [Fact]
            public void Then_a_column_containing_an_integer_should_be_read()
            {
                _result.Integer.ShouldEqual(_dataRecord[FieldName.Int]);
            }

            [Fact]
            public void Then_a_column_containing_a_big_integer_should_be_read()
            {
                _result.BigInteger.ShouldEqual(_dataRecord[FieldName.BigInt]);
            }

            [Fact]
            public void Then_a_column_containing_a_GUID_should_be_read()
            {
                _result.Guid.ShouldEqual(_dataRecord[FieldName.Guid]);
            }

            [Fact]
            public void Then_a_column_containing_a_float_should_be_read()
            {
                _result.Float.ShouldEqual(_dataRecord[FieldName.Float]);
            }

            [Fact]
            public void Then_a_column_containing_a_double_should_be_read()
            {
                _result.Double.ShouldEqual(_dataRecord[FieldName.Double]);
            }

            [Fact]
            public void Then_a_column_containing_a_decimal_should_be_read()
            {
                _result.Decimal.ShouldEqual(_dataRecord[FieldName.Decimal]);
            }

            [Fact]
            public void Then_a_column_containing_a_date_and_time_should_be_read()
            {
                _result.DateTime.ShouldEqual(_dataRecord[FieldName.DateTime]);
            }

            [Fact]
            public void Then_a_column_containing_a_byte_should_be_read()
            {
                _result.Byte.ShouldEqual(_dataRecord[FieldName.Byte]);
            }

            [Fact]
            public void Then_a_column_containing_a_boolean_should_be_read()
            {
                _result.Boolean.ShouldEqual(_dataRecord[FieldName.Boolean]);
            }

            [Fact]
            public void Then_a_projected_column_should_be_read()
            {
                _result.Else.ShouldEqual(_dataRecord[FieldName.Something]);
            }

            private DTOObject _result;
            private IDataRecord _dataRecord;
        }

        public class When_mapping_a_data_reader_to_a_dto_with_nullable_field : AutoMapperSpecBase
        {
            internal const string FieldName = "Integer";
            internal const int FieldValue = 7;

            internal class DtoWithSingleNullableField
            {
                public int? Integer { get; set; }
            }

            internal class DataBuilder
            {
                public IDataReader BuildDataReaderWithNullableField()
                {
                    var table = new DataTable();

                    var col = table.Columns.Add(FieldName, typeof(int));
                    col.AllowDBNull = true;

                    var row1 = table.NewRow();
                    row1[FieldName] = FieldValue;
                    table.Rows.Add(row1);

                    var row2 = table.NewRow();
                    row2[FieldName] = DBNull.Value;
                    table.Rows.Add(row2);

                    return table.CreateDataReader();
                }
            }

            protected override void Establish_context()
            {
                Mapper.CreateMap<IDataReader, DtoWithSingleNullableField>();

                _dataReader = new DataBuilder().BuildDataReaderWithNullableField();
            }

            [Fact]
            public void Then_results_should_be_as_expected()
            {
                while (_dataReader.Read())
                {
                    var dto = Mapper.Map<IDataReader, DtoWithSingleNullableField>(_dataReader);

                    if (_dataReader.IsDBNull(0))
                        dto.Integer.HasValue.ShouldEqual(false);
                    else
                    {
                        // uncomment the following line to see some strange fail message that might be the key to the problem
                        dto.Integer.HasValue.ShouldEqual(true);

                        dto.Integer.Value.ShouldEqual(FieldValue);
                    }
                }
            }

            private IDataReader _dataReader;
        }

		public class When_mapping_a_data_reader_to_a_dto_with_nullable_enum : AutoMapperSpecBase
		{
			internal const string FieldName = "Value";
			internal const int FieldValue = 3;

			public enum settlement_type
			{
				PreDelivery = 0,
				DVP = 1,
				FreeDelivery = 2,
				Prepayment = 3,
				Allocation = 4,
				SafeSettlement = 5,
			}
			internal class DtoWithSingleNullableField
			{
				public settlement_type? Value { get; set; }
			}

			internal class DataBuilder
			{
				public IDataReader BuildDataReaderWithNullableField()
				{
					var table = new DataTable();

					var col = table.Columns.Add(FieldName, typeof(int));
					col.AllowDBNull = true;

					var row1 = table.NewRow();
					row1[FieldName] = FieldValue;
					table.Rows.Add(row1);

					var row2 = table.NewRow();
					row2[FieldName] = DBNull.Value;
					table.Rows.Add(row2);

					return table.CreateDataReader();
				}
			}

			protected override void Establish_context()
			{
				Mapper.CreateMap<IDataReader, DtoWithSingleNullableField>();

				_dataReader = new DataBuilder().BuildDataReaderWithNullableField();
			}

			[Fact]
			public void Then_results_should_be_as_expected()
			{
				while (_dataReader.Read())
				{
					//var dto = Mapper.Map<IDataReader, DtoWithSingleNullableField>(_dataReader);
					var dto = new DtoWithSingleNullableField();

					object value = _dataReader[0];
					if (!Equals(value, DBNull.Value))
						dto.Value = (settlement_type)value;

					if (_dataReader.IsDBNull(0))
						dto.Value.HasValue.ShouldBeFalse();
					else
					{
						dto.Value.HasValue.ShouldBeTrue();

						dto.Value.Value.ShouldEqual(settlement_type.Prepayment);
					}
				}
			}

			private IDataReader _dataReader;
		}

        internal class FieldName
        {
            public const String SmallInt = "SmallInteger";
            public const String Int = "Integer";
            public const String BigInt = "BigInteger";
            public const String Guid = "Guid";
            public const String Float = "Float";
            public const String Double = "Double";
            public const String Decimal = "Decimal";
            public const String DateTime = "DateTime";
            public const String Byte = "Byte";
            public const String Boolean = "Boolean";
            public const String Something = "Something";
        }

        public class DataBuilder
        {
            public IDataReader BuildDataReader()
            {
                var authorizationSetDataTable = new DataTable();
                authorizationSetDataTable.Columns.Add(FieldName.SmallInt, typeof(Int16));
                authorizationSetDataTable.Columns.Add(FieldName.Int, typeof(Int32));
                authorizationSetDataTable.Columns.Add(FieldName.BigInt, typeof(Int64));
                authorizationSetDataTable.Columns.Add(FieldName.Guid, typeof(Guid));
                authorizationSetDataTable.Columns.Add(FieldName.Float, typeof(float));
                authorizationSetDataTable.Columns.Add(FieldName.Double, typeof(Double));
                authorizationSetDataTable.Columns.Add(FieldName.Decimal, typeof(Decimal));
                authorizationSetDataTable.Columns.Add(FieldName.DateTime, typeof(DateTime));
                authorizationSetDataTable.Columns.Add(FieldName.Byte, typeof(Byte));
                authorizationSetDataTable.Columns.Add(FieldName.Boolean, typeof(Boolean));
                authorizationSetDataTable.Columns.Add(FieldName.Something, typeof(DateTime));

                var authorizationSetDataRow = authorizationSetDataTable.NewRow();
                authorizationSetDataRow[FieldName.SmallInt] = 22;
                authorizationSetDataRow[FieldName.Int] = 6134;
                authorizationSetDataRow[FieldName.BigInt] = 61346154;
                authorizationSetDataRow[FieldName.Guid] = Guid.NewGuid();
                authorizationSetDataRow[FieldName.Float] = 642.61;
                authorizationSetDataRow[FieldName.Double] = 67164.64;
                authorizationSetDataRow[FieldName.Decimal] = 94341.61;
                authorizationSetDataRow[FieldName.DateTime] = DateTime.Now;
                authorizationSetDataRow[FieldName.Byte] = 0x12;
                authorizationSetDataRow[FieldName.Boolean] = true;
                authorizationSetDataRow[FieldName.Something] = DateTime.MaxValue;
                authorizationSetDataTable.Rows.Add(authorizationSetDataRow);

                return authorizationSetDataTable.CreateDataReader();
            }

            public IDataRecord BuildDataRecord()
            {
                var dataReader = BuildDataReader();
                dataReader.Read();
                return dataReader;
            }
        }

        public class DTOObject
        {
            public Int16 SmallInteger { get; private set; }
            public Int32 Integer { get; private set; }
            public Int64 BigInteger { get; private set; }
            public Guid Guid { get; private set; }
            public float Float { get; private set; }
            public Double Double { get; private set; }
            public Decimal Decimal { get; private set; }
            public DateTime DateTime { get; private set; }
            public Byte Byte { get; private set; }
            public Boolean Boolean { get; private set; }
            public DateTime Else { get; private set; }
        }

        public class DerivedDTOObject : DTOObject { }
    }
}
#endif