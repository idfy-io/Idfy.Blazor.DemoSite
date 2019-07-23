using System;

namespace Idfy.Blazor.DemoSite.Client.Static
{
    public static class DocNameGenerator
    {
        private static readonly Random r = new Random();
        private static readonly string[] nouns = new string[]
        {
            "Document",
            "Contract",
            "Agreement",
            "File"
        };

        private static readonly string[] adjectives = new string[]
        {
            "Attractive",
            "Bald",
            "Beautiful",
            "Chubby",
            "Clean",
            "Dazzling",
            "Drab",
            "Elegant",
            "Fancy",
            "Fit",
            "Flabby",
            "Glamorous",
            "Gorgeous",
            "Handsome",
            "Long",
            "Magnificent",
            "Muscular",
            "Plain",
            "Plump",
            "Quaint",
            "Scruffy",
            "Shapely",
            "Short",
            "Skinny",
            "Stocky",
            "Ugly",
            "Unkempt",
            "Unsightly"
        };

        public static string GenerateTitle() => $"{adjectives[r.Next(adjectives.Length - 1)]} {nouns[r.Next(nouns.Length - 1)]}";
    }
}
