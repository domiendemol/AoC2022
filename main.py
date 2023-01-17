# This is a sample Python script.

# Press Shift+F10 to execute it or replace it with your code.
# Press Double Shift to search everywhere for classes, files, tool windows, actions, and settings.

from day9 import Day9
from day1 import Day1
from day10 import Day10
from day2 import Day2
from day11 import Day11

import time

def print_hi(name):
    # Use a breakpoint in the code line below to debug your script.
    print(f'Hi, {name}')  # Press Ctrl+8 to toggle the breakpoint.


def day5():
    print("day5")

    # manual input for now, but actually easy to parse as well (to do)
    list1 = ['B', 'V', 'S', 'N', 'T', 'C', 'H', 'Q']
    list2 = ['W', 'D', 'B', 'G']
    list3 = ['F', 'W', 'R', 'T', 'S', 'Q', 'B']
    list4 = ['L', 'G', 'W', 'S', 'Z', 'J', 'D', 'N']
    list5 = ['M', 'P', 'D', 'V', 'F']
    list6 = ['F', 'W', 'J']
    list7 = ['L', 'N', 'Q', 'B', 'J', 'V']
    list8 = ['G', 'T', 'R', 'C', 'J', 'Q', 'S', 'W']
    list9 = ['J', 'S', 'Q', 'C', 'W', 'D', 'M']

    listList = [list1, list2, list3, list4, list5, list6, list7, list8, list9]

    f = open("day5.txt", "r")
    lines = f.readlines()

    for line in lines:
        # print(line, end="")
        parts = line.split(" ")

        for x in range(int(parts[1])):
            # part 1
            listList[int(parts[5])-1].append(listList[int(parts[3])-1].pop())
            # part 2
            # listList[int(parts[5]) - 1].append(listList[int(parts[3]) - 1].pop(-int(parts[1]) + x))

    result = ""
    for n in listList:
        result += n[-1]
    print(f"RESULT: {result}")


def day6():
    q = []
    q2 = []
    f = open("day6.txt", "r")
    lines = f.readlines()

    # loop chars, store last 4 characters
    i = 0
    part1done = False
    part2done = False
    for c in lines[0]:
        q.append(c)
        q2.append(c)
        if len(q) > 4:
            q.pop(0)
        if len(q2) > 14:
            q2.pop(0)
        if len(q) == 4:
            if part1done == False and len(unique(q)) == 4:
                print(f"PART 1 RESULT: " + str(i+1))
                part1done = True
        if len(q2) == 14:
            if part2done == False and len(unique(q2)) == 14:
                print(f"PART 2 RESULT: " + str(i+1))
                part2done = True
        i+=1


def unique(list1):
    x = np.array(list1)
    return np.unique(x)

# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    print_hi('AoC 2022')

    t = time.time_ns()

    # Day1().run()
    # Day9().run()
    # Day10().run()
    Day11().run()
    # Day2().run()

    print(f"Executed in: {(time.time_ns() - t) / 1000000}ms")

# See PyCharm help at https://www.jetbrains.com/help/pycharm/
