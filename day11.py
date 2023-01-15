from types import FunctionType
import re
import math


class Day11:

    monkeys = []

    def run(self):
        self.input()
        self.part1()
        self.input()
        self.part2()

    def part1(self):
        # ROUNDS
        for round in range(0, 20):
            print(f"ROUND {round}----------------------------------")
            for monkey in self.monkeys:
                print(f"TURN {monkey}-------------")
                monkey.turn(self)
        print(self.monkeys)

        self.monkeys.sort(key=get_inspections, reverse=True)
        res = list(self.monkeys[f].inspect_count for f in range(0, 2))
        print(f"PART 1 RESULT: {res[0] * res[1]}")

    def part2(self):
        # ROUNDS
        for round in range(0, 10000):
            print(f"ROUND {round}----------------------------------")
            for monkey in self.monkeys:
                # print(f"TURN {monkey}-------------")
                monkey.turn2(self)
        print(self.monkeys)

        self.monkeys.sort(key=get_inspections, reverse=True)
        res = list(self.monkeys[f].inspect_count for f in range(0, 2))
        print(f"PART 2 RESULT: {res[0] * res[1]}")

    def input(self):
        self.monkeys = []
        # parse input - create monkeys
        f = open("day11.txt", "r")
        lines = f.readlines()
        monkey = Monkey(-1)
        for line in lines:
            # regular exxxxpressions
            mo = re.search('Monkey (\\d):*', line)
            it = re.search('Starting items: (.+)', line)
            op = re.search('Operation: new = (.+)', line)
            te = re.search('Test: divisible by (\\d+)', line)
            rere = re.search('If ([^:]+): throw to monkey (\\d)', line)
            if mo:
                monkey = Monkey(mo.group(1))
                self.monkeys.append(monkey)
            if it:
                for i in it.group(1).split(', '):
                    monkey.items.append(Item(i))
            if op:
                monkey.operation = op.group(1)
            if te:
                monkey.test = int(te.group(1))
            if rere:
                monkey.results[0 if rere.group(1) == 'true' else 1] = int(rere.group(2))
            # monkey = Monkey()
        print(self.monkeys)


class Monkey:
    name = 0
    items = []
    operation = ""
    test = 1
    results = [0, 0]  # where to throw to
    inspect_count = 0
    to_remove = []

    def turn(self, day11):
        print(self.items)
        for i in range(0, len(self.items)):
            item = self.items[i]
            # 1. Worry level operation
            item_val = item.add_op(self.operation, True)
            # 2. Monkey bored: / 3
            item_val = int(item_val/3)
            # self.items[i] = item
            # 3. Test
            if item_val % self.test == 0:
                self.throw(i, day11.monkeys[self.results[0]])
            else:
                self.throw(i, day11.monkeys[self.results[1]])
            self.inspect_count += 1

        # remove thrown items from list
        self.items = []

    def turn2(self, day11):
        # print(self.items)
        for i in range(0, len(self.items)):
            item = self.items[i]
            # 1. Worry level operation
            item.add_op(self.operation, False)
            # item_val = int(item_val)
            # self.items[i] = item
            # 2. Test
            if item.mod(self.test) == 0:
                self.throw(i, day11.monkeys[self.results[0]])
            else:
                self.throw(i, day11.monkeys[self.results[1]])
            self.inspect_count += 1

        # remove thrown items from list
        self.items = []

    def throw(self, item, target):
        # print(f"Throwing item [{item}] to monkey [{target}]")
        # self.to_remove.append(item)  # mark index as to_remove
        target.items.append(self.items[item])

    def __init__(self, name):
        self.name = name
        self.results = [0, 0]
        self.items = []

    def __repr__(self):
        return f"Monkey! Name: {self.name}, Items:{self.items}, Inspections: {self.inspect_count}"


class Item:
    start = 0
    ops = []

    def __init__(self, start):
        self.start = int(start)
        self.ops = []

    def add_op(self, op, calculate):
        self.ops.append(op)

        if calculate:
            val = self.start
            for o in self.ops:
                # function during run-time
                f_code = compile(f"def worry_operation(): return {o}".replace("old", str(val)), "<int>", "exec")
                f_func = FunctionType(f_code.co_consts[0], globals(), "worry_operation")
                val = int(f_func())
                # print(f"Val: {val}")
            return val

    def mod(self, mod):
        val = self.start
        for i in range(0, len(self.ops)):
            op = self.ops[i]
            if op.__contains__("+"):
                val = (val % mod + int(op[-2:]) % mod) % mod
            else:
                op = op.replace("old", str(val))
                index = op.index('*')
                f = op[index + 2:]
                val = (val % mod * int(f) % mod) % mod
        return val

    def __repr__(self):
        return f"Start: {self.start}"


# GLOBAL
def get_inspections(monkey):
    return monkey.inspect_count
