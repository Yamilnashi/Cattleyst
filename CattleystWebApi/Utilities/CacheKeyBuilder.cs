namespace CattleystWebApi.Utilities
{
    public static class CacheKeyBuilder
    {
        private const string GlobalPrefix = "cattleyst_v1_";

        // specific entities
        private const string LocationPrefix = GlobalPrefix + "location_";
        private const string CattlePrefix = GlobalPrefix + "cattle_";

        // no parameters
        public const string AllLocations = LocationPrefix + "all";
        public const string AllCattle = CattlePrefix + "all";

        // dynamic
        public static string LocationById(int id) => $"{LocationPrefix}id_{id}";
        public static string CattleAll() => $"{CattlePrefix}all";
        public static string CattleByLocations(int[]? locationIds)
        {
            if (locationIds == null || 
                locationIds.Length == 0)
            {
                return CattleAll();
            }
            IEnumerable<string> sortedIds = locationIds
                .OrderBy(x => x)
                .Select(x => x.ToString());
            string idsHash = string.Join("_", sortedIds);
            return $"{CattlePrefix}by_locations_{idsHash}";
        }
        public static string LocationPrefixPattern() => $"{LocationPrefix}*";
        public static string CattlePrefixPattern() => $"{CattlePrefix}*";
    }
}
