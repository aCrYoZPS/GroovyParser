class GroovyCollectionDemo {
    
    static void processItems(List<String> items) {
        items.eachWithIndex { item, index ->
            println("Processing item ${index + 1}: $item")
        }
    }
    
    // Filter and transform data 
    static Map<String, Integer> countLetters(List<String> words) {
        words.collectEntries { word ->
            def cleanWord = word.replaceAll(/[^a-zA-Z]/, '')
            [cleanWord, cleanWord.size()]
        }
    }
    
    // Generate a sequence
    static List<Integer> generateFibonacci(int count) {
        def sequence = []
        if (count >= 1) sequence << 0
        if (count >= 2) sequence << 1
        
        (2..<count).each { 
            sequence << sequence[-2] + sequence[-1]
        }
        
        sequence.take(count)
    }
    
    // Process nested data structures
    static void processNestedData(Map<String, List<Number>> data) {
        data.each { category, values ->
            println("Category: ${category}")
            println("  Sum: ${values.sum()}")
            println("  Avg: ${values.sum() / values.size()}")
            println("  Max: ${values.max()}")
        }
    }
    
    // Main execution
    static void main(String[] args) {
        def sampleItems = ['apple', 'banana', 'cherry', 'date']
        def sampleData = [
            'Temperatures': [72, 68, 75, 80, 77],
            'Prices': [12.99, 9.99, 15.50, 8.75]
        ]
        
        println("=== Processing Items ===")
        processItems(sampleItems)
        
        println("\n=== Counting Letters ===")
        def letterCounts = countLetters(sampleItems)
        letterCounts.each { word, count ->
            println("Word ${word}: ${count} letters")
        }
        
        println "\n=== Fibonacci Sequence ==="
        def fibSequence = generateFibonacci(10)
        println("First 10 Fibonacci numbers: ${fibSequence}")
        
        println("\n=== Processing Nested Data ===")
        processNestedData(sampleData)
        
        println("\n=== Additional Operations ===")
        def combined = sampleItems.inject([]) { acc, val ->
            acc << val.toUpperCase()
        }
        println("Uppercase items: ${combined}")
        
        def grouped = sampleItems.groupBy { it.size() }
        println("Grouped by length: ${grouped}")
    
        println("\n=== Number Operations ===")
        def scanner = new Scanner(System.in)
        
        print("Enter a num: ")
        def userNumber = scanner.nextInt()
        
        def factorial = (1..userNumber).inject(1) { acc, val -> acc * val }
        println("Factorial ${userNumber}! = ${factorial}")
        
        def fibUpTo = generateFibonacci(20).findAll { it <= userNumber }
        println("Fibonacci before ${userNumber}: ${fibUpTo}")

        println("Binary representation: ${Integer.toBinaryString(userNumber)}")
        
    }
}

// Run the script
GroovyCollectionDemo.main()