using Amazon.DynamoDBv2.DocumentModel;

namespace BananaTracks.Domain.Configuration;

public class DateTimeUtcConverter : IPropertyConverter
{
	public DynamoDBEntry ToEntry(object value)
	{
		return (DateTime)value;
	}

	public object FromEntry(DynamoDBEntry entry)
	{
		var dateTime = entry.AsDateTime();
		return dateTime.ToUniversalTime();
	}
}
