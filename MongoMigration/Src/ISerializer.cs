namespace MongoMigration
{
    public interface ISerializer<TNominal>
    {
        byte[] Serialize(TNominal element);

        TNominal Deserialize(byte[] data);
    }
}