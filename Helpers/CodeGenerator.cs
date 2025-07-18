namespace ParkNet.Helpers
{
    public class CodeGenerator
    {
        private static readonly string alphabet = new string(
                  Enumerable.Range('A', 26).Select(c => (char)c)
                  .Concat(Enumerable.Range('A', 26).Select(c => (char)c)).ToArray()
              );

        public static string GetLetterBasedOnNumber(int floor, int spotIndex)
        {
            if (floor >= alphabet.Length || spotIndex >= alphabet.Length)
                throw new Exception("Letter overflow");

            string floorCode = GetPosition(floor);
            string spotCode = spotIndex > 26 ? GetPosition(spotIndex) : spotIndex.ToString();

            return $"{floorCode}{spotCode}";
        }

        private static string GetPosition(int pos)
        {
            return alphabet[pos].ToString();
        }
    }
}
