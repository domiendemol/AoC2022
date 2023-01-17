class TreeNode:
    def __init__(self, name):
        self.name = name
        self.size = 0
        self.parent = None
        self.children = []

    def add_size(self, size):
        print(f"--> Added {size} to {self.name}")
        self.size += size
        if self.parent:
            self.parent.add_size(size)

    def get_child(self, name):
        for child in self.children:
            if child.name == name:
                return child

        # not found, create new node
        nw = TreeNode(name)
        self.children.append(nw)
        nw.parent = self
        return nw

    def print_size(self, min_size):
        if self.size > min_size:
            print(f"{self.name} - {self.size}")
        for child in self.children:
            child.print_size(min_size)
