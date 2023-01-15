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
