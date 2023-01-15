class Day10:

    tick = 0
    signal = 1
    results = [] # contains the signal at the end of the tick (not in the middle!)

    def run(self):
        f = open("day10.txt", "r")
        lines = f.readlines()

        for line in lines:
            parts = line.replace("\n", "").split(" ")
            self.assemble(parts)

        result = sum(self.results[a-1] * (a+1) for a in range(19, 220, 40))
        print(f"PART 1 RESULT: {result}")

        # rendering
        print("PART 2 RESULT:")
        for i in range(0, 6):
            line = ""
            for j in range(0, 40):
                if abs(j - self.results[i*40+j-1]) <= 1:
                    line += "#"
                else:
                    line += "."
            print(line)

    def assemble(self, instruction):
        if self.tick > 200:
            print(f"{self.tick} - {self.signal}")
        self.results.append(self.signal)
        self.tick += 1
        if instruction[0] == "addx":
            self.signal += int(instruction[1])
            print(f"Signal: {self.signal}")
            self.results.append(self.signal)
            self.tick += 1
