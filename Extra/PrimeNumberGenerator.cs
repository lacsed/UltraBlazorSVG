using System;
using System.Collections.Generic;

public class PrimeNumberGenerator
{
    public static List<int> GeneratePrimes(int n)
    {
        List<int> primes = new List<int>();

        int number = 2;
        while (primes.Count < n)
        {
            if (IsPrime(number))
            {
                primes.Add(number);
            }
            number++;
        }

        return primes;
    }

    private static bool IsPrime(int number)
    {
        if (number < 2)
        {
            return false;
        }

        for (int i = 2; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0)
            {
                return false;
            }
        }

        return true;
    }
}