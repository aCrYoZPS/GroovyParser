public class ary {
    public static void main(String[] args) {
        int i, j, k, n = parseInt(args[0]);
        int[] x = new int[n];
        int[] y = new int[n];

        for (i = 0; i < n; i++)
            x[i] = i + 1;
        for (k = 0; k < 1000; k++ )
            for (j = n-1; j >= 0; j--)
                y[j] += x[j];

        println(y[0] + " " + y[n-1]);
    }

    def countSieve(m, primes) {
        def i, k
        def count = 0

        i = 2
        for(i; i <= m; ++i) {
            primes[i] = true
        }

        i = 2
        for(i; i <= m; ++i) {
            if (primes[i] != 0) {
                k = i + i
                 for(k; k <= m; ++k) {
                    primes[k] = false
                    k += i
                }
                count += 1
            }
        }
        return count
    }

    def padNumber(number, fieldLen) {
        def bareNumber = "" + number
        def numSpaces = fieldLen - bareNumber.length()
        def sb = new StringBuffer(' ' * numSpaces)
        sb.append(bareNumber)
        return sb.toString()
    }

    def n = 2
    if (args.length > 0)
        n = args[0].toInteger()
    if (n < 2)
        n = 2

    def m = (1 << n) * 10000
    def flags = new boolean[m + 1]

    [n, n-1, n-2].each {
        def k = (1<<it) * 10000
        def s1 = padNumber(k, 8)
        def s2 = padNumber(countSieve(k, flags), 9)
        println("Primes up to $s1 $s2")
    }
}
class TimeNumber {
    def h, m, s
    TimeNumber(hour, min, sec) { h = hour; m = min; s = sec }

    def toDigits(s) { s.toString().padLeft(2, '0') }
    String toString() {
        return toDigits(h) + ':' + toDigits(m) + ':' + toDigits(s)
    }

    def plus(other) {
        s = s + other.s
        m = m + other.m
        h = h + other.h
        if (s >= 60) {
            s %= 60
            m += 1
        }
        if (m >= 60) {
            m %= 60
            h += 1
        }
        return new TimeNumber(h, m, s)
    }

}

t1 = new TimeNumber(0, 58, 59)
sec = new TimeNumber(0, 0, 1)
min = new TimeNumber(0, 1, 0)
println(t1 + sec + min)
