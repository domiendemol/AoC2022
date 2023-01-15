class Day2:
    scores = {'X': 1, 'Y': 2, 'Z': 3}
    target_scores = {'X': 0, 'Y': 3, 'Z': 6}

    def run(self):
        f = open("day2.txt", "r")
        lines = f.readlines()

        result = sum(self.get_score(f.replace("\n", "").split(" ")) for f in lines)
        print(f"PART 1 RESULT: {result}")

        result = sum(self.get_other_score(f.replace("\n", "").split(" ")) for f in lines)
        print(f"PART 2 RESULT: {result}")

    def get_score(self, signs):
            opponent = signs[0]
            player = signs[1]
            score = self.scores[player]
            if (opponent == 'A' and player == 'Y') \
                    or (opponent == 'B' and player == 'Z') \
                    or (opponent == 'C' and player == 'X'):
                score += 6
            elif (opponent == 'A' and player == 'X') \
                    or (opponent == 'B' and player == 'Y') \
                    or (opponent == 'C' and player == 'Z'):
                score += 3
            return score

    def get_other_score(self, signs):
            opponent = ord(signs[0])
            target = signs[1]
            score = self.target_scores[target]
            if target == 'X':  # lose
                player = chr(ord('X') + (opponent - ord('A') - 1) % 3)
            if target == 'Y':  # draw
                player = chr(ord('X') + opponent - ord('A'))
            elif target == 'Z':  # win
                player = chr(ord('X') + (opponent - ord('A') + 1) % 3)
            score += self.scores[player]

            return score