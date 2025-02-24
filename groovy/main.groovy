class Sort {
    def quickSort(int[] array, int left, int right) {
        if (left < right) {
            int pivotIndex = partition(array, left, right)
            quickSort(array, left, pivotIndex - 1)
            quickSort(array, pivotIndex + 1, right)
        }
    }
    
    def partition(int[] array, int left, int right) {
        int pivot = array[right]
        int i = left - 1
        for (int j = left; j < right; j++) {
            if (array[j] <= pivot) {
                i++
                swap(array, i, j)
            }
        }
        swap(array, i + 1, right)
        return i + 1
    }
    
    def bubbleSort(int[] array) {
        int n = array.length
        boolean swapped
        do {
            swapped = false
            for (int i = 1; i < n; i++) {
                if (array[i - 1] > array[i]) {
                    swap(array, i - 1, i)
                    swapped = true
                }
            }
            n--
        } while (swapped)
    }
    
    def interpolationSort(int[] array) {
        int n = array.length
        for (int i = 1; i < n; i++) {
            int key = array[i]
            int j = i - 1
            while (j >= 0 && array[j] > key) {
                array[j + 1] = array[j]
                j--
            }
            array[j + 1] = key
        }
    }
    
    def heapSort(int[] array) {
        int n = array.length
        for (int i = n / 2 - 1; i >= 0; i--) {
            heapify(array, n, i)
        }
        for (int i = n - 1; i > 0; i--) {
            swap(array, 0, i)
            heapify(array, i, 0)
        }
    }
    
    def heapify(int[] array, int n, int i) {
        int largest = i
        int left = 2 * i + 1
        int right = 2 * i + 2
        
        if (left < n && array[left] > array[largest]) {
            largest = left
        }
        if (right < n && array[right] > array[largest]) {
            largest = right
        }
        if (largest != i) {
            swap(array, i, largest)
            heapify(array, n, largest)
        }
    }
    
    def selectionSort(int[] array) {
        int n = array.length
        for (int i = 0; i < n - 1; i++) {
            int minIndex = i
            for (int j = i + 1; j < n; j++) {
                if (array[j] < array[minIndex]) {
                    minIndex = j
                }
            }
            if (minIndex != i) {
                swap(array, i, minIndex)
            }
        }
    }
    
    void mergeSort(int[] array, int left, int right) {
        if (left < right) {
            int mid = left + (right - left) / 2
            mergeSort(array, left, mid)
            mergeSort(array, mid + 1, right)
            merge(array, left, mid, right)
        }
    }
    
    void merge(int[] array, int left, int mid, int right) {
        def leftArray = Arrays.copyOfRange(array, left, mid + 1)
        def rightArray = Arrays.copyOfRange(array, mid + 1, right + 1)
        
        int i = 0, j = 0, k = left
        while (i < leftArray.length && j < rightArray.length) {
            if (leftArray[i] <= rightArray[j]) {
                array[k++] = leftArray[i++]
            } else {
                array[k++] = rightArray[j++]
            }
        }
        while (i < leftArray.length) {
            array[k++] = leftArray[i++]
        }
        while (j < rightArray.length) {
            array[k++] = rightArray[j++]
        }
    }
    
    def swap(int[] array, int i, int j) {
        if (i != j) {
            array[i] = array[i] ^ array[j]
            array[j] = array[i] ^ array[j]
            array[i] = array[i] ^ array[j]
        }
    }
    
    boolean isSortedAscending(int[] array) {
        for (int i = 1; i < array.length; i++) {
            if (array[i - 1] > array[i]) {
                return false
            }
        }
        return true
    }
    
    def isSortedDescending(int[] array) {
        for (int i = 1; i < array.length; i++) {
            if (array[i - 1] < array[i]) {
                return false
            }
        }
        return true
    }
    
    def findMinMax(arr) {
        if (arr == null || arr.isEmpty()) {
            return [min: null, max: null]
        }

        def min = arr[0]
        def max = arr[0]

        arr.each { num ->
            if (num < min) {
                min = num
            }
            if (num > max) {
                max = num
            }
        }

        return [min: min, max: max]
    }

     void main(String[] args) {
        def array = [34, 7, 23, 32, 5, 62]
        
        println("Original: ${array}")
        
        def quickSorted = array.clone()
        quickSort(quickSorted, 0, quickSorted.length - 1)
        println("QuickSort: ${quickSorted}")
        
        def bubbleSorted = array.clone()
        bubbleSort(bubbleSorted)
        println("BubbleSort: ${bubbleSorted}")
        
        def interpolationSorted = array.clone()
        interpolationSort(interpolationSorted)
        println("InterpolationSort: ${interpolationSorted}")
        
        def heapSorted = array.clone()
        heapSort(heapSorted)
        println("HeapSort: ${heapSorted}")
        
        def selectionSorted = array.clone()
        selectionSort(selectionSorted)
        println("SelectionSort: ${selectionSorted}")
        
        def mergeSorted = array.clone()
        mergeSort(mergeSorted, 0, mergeSorted.length - 1)
        println("MergeSort: ${mergeSorted}")
        
        println("Is Sorted Ascending: ${isSortedAscending(array)}")
        println("Is Sorted Descending: ${isSortedDescending(array)}")

        def array = [5, 2, 9, 1, 7]
        def result = findMinMax(array)
        println "Min: ${result.min}, Max: ${result.max}"
    }
