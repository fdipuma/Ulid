using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace UlidTests
{
    public class UlidJsonConverterTest
    {
        class TestSerializationClass
        {
            public Ulid value { get; set; }
        }

        JsonSerializerOptions GetDefaultOptions()
        {
            return new JsonSerializerOptions()
            {
                Converters =
                {
                    new Cysharp.Serialization.Json.UlidJsonConverter()
                }
            };
        }

        JsonSerializerOptions GetLowercaseOptions()
        {
            return new JsonSerializerOptions()
            {
                Converters =
                {
                    new Cysharp.Serialization.Json.UlidJsonConverter(outputLowercase: true)
                }
            };
        }

        [Fact]
        public void DeserializeTest()
        {
            var target = Ulid.NewUlid();
            var src = $"{{\"value\": \"{target.ToString()}\"}}";

            var parsed = JsonSerializer.Deserialize<TestSerializationClass>(src, GetDefaultOptions());
            parsed.value.Should().BeEquivalentTo(target, "JSON deserialization should parse string as Ulid");
        }

        [Fact]
        public void DeserializeExceptionTest()
        {
            var target = Ulid.NewUlid();
            var src = $"{{\"value\": \"{target.ToString().Substring(1)}\"}}";
            try
            {
                var parsed = JsonSerializer.Deserialize<TestSerializationClass>(src, GetDefaultOptions());
                throw new Exception("Test should fail here: no exception were thrown");
            }
            catch (JsonException)
            {
                // silentlly success
            }
            catch (Exception e)
            {
                throw new Exception($"Test should fail here: Got exception {e}");
            }

        }

        [Fact]
        public void SerializeTest()
        {
            var groundTruth = new TestSerializationClass()
            {
                value = Ulid.NewUlid()
            };

            var serialized = JsonSerializer.Serialize(groundTruth, GetDefaultOptions());
            var deserialized = JsonSerializer.Deserialize<TestSerializationClass>(serialized, GetDefaultOptions());
            deserialized.value.Should().BeEquivalentTo(groundTruth.value, "JSON serialize roundtrip");
        }

        [Fact]
        public void SerializeLowercaseTest()
        {
            var groundTruth = new TestSerializationClass()
            {
                value = Ulid.NewUlid()
            };

            var expectedJson = $"{{\"value\":\"{groundTruth.value.ToString().ToLowerInvariant()}\"}}";
            
            var serialized = JsonSerializer.Serialize(groundTruth, GetDefaultOptions());
            serialized.Should().BeEquivalentTo(expectedJson, "Serialize as lowercase");
            var deserialized = JsonSerializer.Deserialize<TestSerializationClass>(serialized, GetLowercaseOptions());
            deserialized.value.Should().BeEquivalentTo(groundTruth.value, "JSON serialize roundtrip");
        }

        [Fact]
        public void WithtoutOptionsTest()
        {
            var groundTruth = new TestSerializationClass()
            {
                value = Ulid.NewUlid()
            };

            var serialized = JsonSerializer.Serialize(groundTruth);
            var deserialized = JsonSerializer.Deserialize<TestSerializationClass>(serialized);
            deserialized.value.Should().BeEquivalentTo(groundTruth.value, "JSON serialize roundtrip");
        }
    }
}
