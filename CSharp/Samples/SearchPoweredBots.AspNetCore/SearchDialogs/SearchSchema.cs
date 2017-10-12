﻿using System;
using System.Collections.Generic;
using Microsoft.Azure.Search.Models;

namespace Microsoft.Bot.Sample.AspNetCore.SearchDialogs
{
    public enum PreferredFilter { None, MinValue, MaxValue, RangeMin, RangeMax, Range };

    [Serializable]
    public class SearchField
    {
        // Fields from Azure Search
        public string Name;
        public Type Type;
        public bool IsFacetable;
        public bool IsFilterable;
        public bool IsKey;
        public bool IsRetrievable;
        public bool IsSearchable;
        public bool IsSortable;

        // Fields to control
        public PreferredFilter FilterPreference;
    }

    [Serializable]
    public class SearchSchema
    {
        public SearchSchema()
        {
        }

        public Dictionary<string, SearchField> Fields = new Dictionary<string, SearchField>();
    }

    public static partial class Extensions
    {
        public static SearchField ToSearchField(this Field field)
        {
            Type type;
            if (field.Type == DataType.Boolean) type = typeof(Boolean);
            else if (field.Type == DataType.DateTimeOffset) type = typeof(DateTime);
            else if (field.Type == DataType.Double) type = typeof(double);
            else if (field.Type == DataType.Int32) type = typeof(Int32);
            else if (field.Type == DataType.Int64) type = typeof(Int64);
            else if (field.Type == DataType.String) type = typeof(string);
            else if (field.Type == DataType.GeographyPoint) type = typeof(Spatial.GeographyPoint);
            // Azure Search DataType objects don't follow value comparisons, so use overloaded string conversion operator to be a consistent representation
            else if ((string)field.Type == (string)DataType.Collection(DataType.String)) type = typeof(string[]);
            else
            {
                throw new ArgumentException($"Cannot map {field.Type} to a C# type");
            }
            return new SearchField()
            {
                Name = field.Name,
                Type = type,
                IsFacetable = field.IsFacetable,
                IsFilterable = field.IsFilterable,
                IsKey = field.IsKey,
                IsRetrievable = field.IsRetrievable,
                IsSearchable = field.IsSearchable,
                IsSortable = field.IsSortable
            };
        }

        public static SearchSchema AddFields(this SearchSchema schema, IEnumerable<Field> fields)
        {
            foreach(var field in fields)
            {
                schema.Fields[field.Name] = field.ToSearchField();
            }
            return schema;
        }
    }
}

