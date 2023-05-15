def merge(left_list, right_list) :
    sorted = ( )
    left_index = right_index = 0
    left_length , right_length = len(left_list) , len(right_list)
    for _ in range(left_length + right_length) :
        if left_index < left_length and right_index < right_length :
            if left_list(left_index) <= right_list(right_index) :
                sorted.append(left_list ( left_index ) )
                left_index = left_index + 1
            else :
                sorted.append(right_list ( right_index ) )
                right_index = right_index + 1
        elif left_index == left_length :
            sorted.append(right_list ( right_index ) )
            right_index = right_index + 1
        elif right_index == right_length :
            sorted.append(left_list ( left_index ) )
            left_index = left_index + 1
    return sorted

def merge_sort(nums) :
    if len(nums) <= 1 :
        return nums
    mid = int(len(nums) / 2)
    left_list = merge_sort(nums ( :mid ) )
    right_list = merge_sort(nums ( mid: ) )
    return merge(left_list, right_list)

random_nums = (64, 83, 50, 168, 5)
random_nums = merge_sort(random_nums)
print('Сортировка слиянием')
print(random_nums)