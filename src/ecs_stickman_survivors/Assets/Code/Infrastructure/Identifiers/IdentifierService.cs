namespace Code.Infrastructure.Identifiers
{
    public class IdentifierService : IIdentifierService
    {
        private int _lastId = 1;

        public int Next() =>
            ++_lastId;
    }
}