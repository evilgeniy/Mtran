def fib_numbers(input_number) :
    fib_number1 = 1
    fib_number2 = 2
    if input_number < 3 :
        print("")
    print (fib_number1, fib_number2, end=" ")
    for i in range(3, input_number + 1) :
        print (fib_number1 + fib_number2, end=" ")
        temp = fib_number1
        fib_number1 = fib_number2
        fib_number2 = temp + fib_number1
fib_numbers (100)