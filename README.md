# multi-agent sequential learning combating adversarial procedural generation

We were inspired by the game plants versus zombies. We built a board with ten lanes, each 30 tiles long.

Four defenders were located on the first through fourth tiles. Each tick, each defender can move left, move right, shoot, heal, use a special ability, or do nothing. Each defender has an amount of energy they can spend on actions, which replenishes over time up to a maximum.

An attacker is located at the opposite end of the board, 26 tiles from the closest of the four defenders. Each tick, the attacker can either generate a spawn or do nothing. If the attacker generates a spawn, it decides on how much life, damage, speed, range, regen, leach, physical penetration, magic penetration, physical defense, and magic defense to give it and decides its damage type. The attacker can spend energy on generating spawns, which recovers over time. The attacker also has a maximum amount of energy, which very slowly increases over time. The stronger the spawn, the more energy it will cost.

The spawns follow a very simple set of rules. If a defender is within their range, they shoot. Otherwise they move forwards. If a spawn reaches the edge of the board, past the defenders, or if a defender is defeated, then the attacker wins. The defender’ goal is to last as long as they can before a spawn passes them or a defender is defeated.

The defenders learned to move to where a spawn is and shoot it. However, they were often wasteful and would frequently still shoot when no spawn was in their lane.

The attacker learned to generate spawns in certain patterns. It would frequently generate a spawn that was fast with few other traits in an attempt to have it race past the defenders. The attacker would also sometimes generate two spawns immediately after one another in the same lane so the front spawn would block attacks from the defender from hitting the back spawn.
