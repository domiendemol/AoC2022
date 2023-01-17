class Day8:

    trees = []

    def run(self):
        f = open("day8.txt", "r")
        lines = f.readlines()

        # parse input
        for line in lines:
            tree_line = []
            for tree in line:
                if not tree == '\n':
                    tree_line.append(tree)
            self.trees.append(tree_line)

        print (self.trees)

        # find free trees
        result = 0
        max = 0
        for x in range(0, len(self.trees)):
            for y in range(0, len(self.trees[x])):
                if self.is_visible(x, y):
                    result += 1
                    sv = self.get_scenic_value(x, y)
                    if sv > max:
                        max = sv

        print(f"RESULT PART 1: {result}")
        print(f"RESULT PART 2: {max}")


    def is_visible(self, row, col):
        if row == 0 or col == 0 or row == len(self.trees)-1 or col == len(self.trees[row])-1:
            # print(f"{row} - {col}")
            return True

        height = self.trees[row][col]

        # go through trees in 4 directions, if we find a tree that's higher: not visible
        # left
        vis_left = True
        for i in range(0, col):
            if self.trees[row][i] >= height:
                vis_left = False
                break
        # right
        vis_right = True
        for i in range(col+1, len(self.trees[row])):
            if self.trees[row][i] >= height:
                vis_right = False
                break
        # up
        vis_up = True
        for i in range(0, row):
            if self.trees[i][col] >= height:
                vis_up = False
                break
        # down
        vis_down = True
        for i in range(row+1, len(self.trees)):
            if self.trees[i][col] >= height:
                vis_down = False
                break
        # print(f"{row},{col} | {vis_up} & {vis_down}")

        if vis_up or vis_down or vis_left or vis_right:
            return True
        else:
            return False

    def get_scenic_value(self, row, col):
        if row == 0 or col == 0 or row == len(self.trees)-1 or col == len(self.trees[row])-1:
            return 0

        height = self.trees[row][col]
        print(f"{row},{col}")
        # go through trees in 4 directions, if we find a tree that's higher: not visible
        # left
        vis_left = col
        for i in range(col-1, -1, -1):
            print(i)
            print(self.trees[row][i])
            if self.trees[row][i] >= height:
                vis_left = abs(i-col)
                break
        # right
        vis_right = len(self.trees[row])-col-1
        for i in range(col+1, len(self.trees[row])):
            if self.trees[row][i] >= height:
                vis_right = abs(i-col)
                break
        # up
        vis_up = row
        for i in range(row-1, -1, -1):
            if self.trees[i][col] >= height:
                vis_up = abs(i-row)
                break
        # down
        vis_down = len(self.trees)-row-1
        for i in range(row+1, len(self.trees)):
            if self.trees[i][col] >= height:
                vis_down = abs(i-row)
                break
        print(f"{row},{col} - {vis_left} & {vis_right}")
        print(f"{row},{col} | {vis_up} & {vis_down}")
        return vis_left * vis_right * vis_up * vis_down

