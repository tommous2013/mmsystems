using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhoneApp.Models
{
    class Shuffle
    {
        private static Random Random = new Random();
        public static void ShuffleIt<T>(T[] array, int seed)
        {
            Random Random = new Random(seed);
            for (int i = array.Length; i > 1; i--)
            {
                int j = Random.Next(i);
                T temp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = temp;
            }
        }
    }
}
