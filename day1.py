from itertools import islice


class Day1:
    cals = [0]

    def run(self):
        f = open("day1.txt", "r")
        lines = f.readlines()

        for line in lines:
            line = line.replace("\n", "")
            if len(line) == 0:
                self.cals.append(0)
            else:
                self.cals[len(self.cals)-1] += int(line)

        print(f"RESULT 1: {max(self.cals)}")
        self.cals.sort(reverse=True)
        print(f"RESULT 2: {sum(f for f in list(islice(self.cals, 3)))}")

