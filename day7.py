from treenode import TreeNode
import numpy as np


class Day7:

    biggest_arr = []

    def run(self):
        f = open("day7.txt", "r")
        lines = f.readlines()

        root = TreeNode("")
        current_node = root

        for line in lines:
            print(line, end="")
            if line.startswith("$ cd"):
                parts = line.split(" ")
                dir = parts[-1]
                dir = dir[0:len(dir)-1]  # cut off last char (newline)
                print("CD " + dir)
                if dir == "/":
                    # do nothing
                    print("root")
                elif dir == "..":
                    # go up a level
                    current_node = current_node.parent
                else:
                    # go in subfolder
                    # create if necessary
                    current_node = current_node.get_child(dir)
            elif not line.startswith("$"):
                parts = line.split(" ")
                if parts[0] == "dir":
                    current_node.get_child(parts[1])
                else:
                    current_node.add_size(int(parts[0]))

        # done processing, now find the biggest folders
        self.find_large_node(root, 100000)
        total = 0
        for a in self.biggest_arr:
            if not a[0] == "":
                total += a[1]

        print(f"RESULT PART 1: {total}")

        # repeat but with larger numbers
        self.biggest_arr = []
        self.find_large_node(root, 3000000)

        # Let's use a as the variable name so we don't override the list keyword
        a = np.array(self.biggest_arr)
        # Need to convert values to int, because they are being casted as string
        # in the original array (arrays only support one data type)
        key_values = a[:, 1].astype(int)
        # Use argsort to get a sort index, then reverse the order to make it descending
        index_array = np.argsort(key_values)[::1]  # -1 reverses the order

        # print(a[index_array])
        print(f"FULL SIZE: {root.size}")
        print(f"FREE SIZE: {70000000 - root.size}")

        # now loop and find the smallest dir to delete
        for dir in a[index_array]:
            # print(f"{int(dir[1])} | {(dir[0])} | {70000000 - root.size + int(dir[1])}")
            if 70000000 - root.size + int(dir[1]) >= 30000000:
                print(f"RESULT PART 2: {dir[1]}")
                break

    def find_large_node(self, node, limit):
        if node.size <= limit:
            self.biggest_arr.append([node.name, node.size])

        for child in node.children:
            self.find_large_node(child, limit)
