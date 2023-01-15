class Day9:

    knots = [(0, 0), (0, 0)]  # list of tuples
    visited = set()

    def run(self):
        f = open("day9.txt", "r")
        lines = f.readlines()

        # add start position as visited position
        self.visited.update([(0, 0)])

        self.part(lines)
        print(f"RESULT 1: {len(self.visited)}")

        # reset
        self.knots = [(0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0)]
        self.visited = set()

        self.part(lines)
        print(f"RESULT 2: {len(self.visited)}")
        # print(self.visited)

    def part(self, lines):
        for line in lines:
            parts = line.replace("\n", "").split(' ')
            for x in range(0, int(parts[1])):
                self.move_head(parts[0])

    def move_head(self, direction):
        # move head
        head = self.knots[0]
        if direction == 'U':
            head = (head[0], head[1]+1)
        elif direction == 'D':
            head = (head[0], head[1]-1)
        elif direction == 'R':
            head = (head[0]+1, head[1])
        elif direction == 'L':
            head = (head[0]-1, head[1])
        self.knots[0] = head
        # move knots
        for k in range(1, len(self.knots)):
            self.knots[k] = self.move_knot(self.knots[k], self.knots[k-1])
            if k == len(self.knots)-1:
                self.visited.update([self.knots[k]])

    def move_knot(self, knot, prev_knot):
        # move tail
        move_x = prev_knot[0] - knot[0]
        move_y = prev_knot[1] - knot[1]
        if abs(move_x) > 1 or abs(move_y) > 1:
            # normalize movements
            if move_x != 0:
                move_x /= abs(move_x)
            if move_y != 0:
                move_y /= abs(move_y)
            knot = (int(knot[0] + move_x), int(knot[1] + move_y))
            # print(self.visited)
        return knot
