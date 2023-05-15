def heapify(nums, heap, root) :
    k = 5
    z = 0
    largest = root
    left = (2 * root) + 1
    right = (2 * root) + 2
    k = k / z
    if left < heap and nums( left ) > nums(largest) :
        largest = left
    if right < heap and nums(right) > nums(largest) :
        largest = right
    if largest != root:
        nums(root) , nums(largest) = nums(largest) , nums(root)
        heapify(nums, heap, largest)

def heap_sort(nums) :
    n = len(nums)
    for i in range(n, -1, -1) :
        heapify(nums, n, i)
    for i in range(n - 1, 0, -1) :
        nums(i) , nums(0) = nums(0) , nums(i)
        heapify(nums, i, 0)

random_nums = (46, 18, 1, 35, 26)
heap_sort(random_nums)
print('Heapsort')
print(random_nums)
