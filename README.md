# The Puzzle Block Coding Challenge
<img align="right" width="337" height="330" alt="Block Puzzle Console" src="./BlockPuzzleConsole.jpg">

Welcome to the Puzzle Block Challenge. This challenge is based on the [Block Puzzle](https://play.google.com/store/apps/details?id=com.differencetenderwhite.skirt) game. 

The purpose of this challenge is to code a new implementation of the `IPlayer` interface, which will beat all other 
implementations by gaining the highest score. 

## The Rules

* You may alter the implementation of the game only for the purpose of supporting your `IPlayer` implementation
* Fork the main branch and push your implementation there. The implementation will be compared to master to make sure no cheats were introduced
* You may use one of the built-in implementation of `IPlayer`, including `SingleStepGreedyPlayer`, `MultiFactorSingleStepPlayer` or `FullEvalPlayerBase`. 
* You may alter one of these implementations completely. 

## The Challenge

In the challenge, the main measure is the final score. Each implementation will be executed 3 times. Only the highest score will be considered. 
The winner is the implementation that reaches the highest score. If two competing implementations get to the same final score, the one that ran for the shortest time wins. 
Each run will be executed using a predetermined seed, so that all implementation will be executed 3 times with 3 different seeds. 

## The Interface

```csharp
interface IPlayer
{
    void MakeAMove(
      out int shapeId, 
      out string placement, 
      Board board, 
      IDictionary<int, Shape> shapes, 
      IGameDrawer renderer);
}
```

The only method that needs to be implemented is `MakeAMove()`. This method is called by the game contiounsly, as long as it is still possible to place a `Shape` on the game board. 
The implementation receives a `board` object, as well as an `IDictionary<int, Shape> shapes`, and needs to output the chosen `shapeId` to place and a `placement` string descriptor that specifies where to place the `shape` on the board. 

The method receives the following parameters:

* `out int shapeId`: The implementation must output a value between 1 and 3 to the caller, which will indicate which shape needs to be placed in this turn. Use the `key` from the `shapes` dictionary to determine the `shapeId`
* `out string placement`: The implementaiton must output a 2 char string indicating the location to place the shape. The first char should be alphanumeric 'a'-'h' and the second char should be numberic '1'-'8'. For example, return `b4` to place the shape starting from the upper-left corner in b4.
* `Board board`: The `board` object includes all the information about the current state of the `board`, as well as different methods to test and manipulate the board. 
* `IDictionary<int, Shape> shapes`: A dictionary of the currently available shapes, that needs to be placed in this turn. The dictionary maps between the `int shapeId` and the actual `shape` object.
* `IGameDrawer renderer`: Can be used to render output to the Console using the `ConsoleGameDrawer` implementation

## How to implement the Player

### From Scratch

You can start from scratch and simply create a new `class` that implements `IPlayer`. As described above, you will simply need to implement the `MakeAMove()` method, which will be called by the game continously until it is no longer possible to place any of the remaining shapes on the board. 

It is recommended that you place your implementation in the Players folder.

For reference, see the implementation of `ManualPlayer` that reads input from the user via the `Console`. 

### Single Step Greedy

Alternatively, you can inherit from the `abstract` `SingleStepGreedyPlayer` class. 

In this implementation, the `MakeAMove()` method iterates over all possible shapes in any specific turn, and attempts to place it in any possible location on the board. For each possible placement, the implementation calls the `Gain()` abstract method, which you need to implement with your own logic of calculating the <i>gain</i> of that specific placement. Then that gain is compared to the last know <i>best</i> gain, using the implementation of `Eval()`. In `Eval()` you can specify how to evaluate the current gain with the last know best gain. If the new gain is "better", it will become the last know best gain. 

```csharp
public void MakeAMove(out int shapeId, out string placement, Board board, 
    IDictionary<int, Shape> shapes, IGameDrawer renderer)
{
    placement = "";
    shapeId = 0;
    var valGain = InitVal();
    foreach (var shape in shapes)
    {
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                var newBoard = new Board(board);
                var curPlacement = "" + (char)(97 + x) + (char)(49 + y);
                if (newBoard.TryPlace(shape.Value, curPlacement))
                {
                    var curValGain = Gain(newBoard, board);
                    if (Eval(curValGain, valGain))
                    {
                        valGain = curValGain;
                        placement = curPlacement;
                        shapeId = shape.Key;
                    }
                }
            }
    }
}
```

As an example, see below the implementation of `Gain()` and `Eval()` for a `Player` that tries to maximize the `Score` for each placement:

```csharp
class ScoreSingleStepGreedyPlayer : SingleStepGreedyPlayer
{
    override public bool Eval(int curGain, int gain)
    {
        return (curGain > gain);
    }

    override public int Gain(Board newBoard, Board board)
    {
        return newBoard.Score - board.Score;
    }
}
```

The advantage of this implementation is that it is relatively fast, but the disadvantage is that it only evaluates one step at a time. 

### Full Evaluation

To do a full analysis of all possible placements given all 3 shapes in each turn, you can derive your implementation from `FullEvalPlayerBase`. This implementation recursively goes over all possible placements for all 3 shapes in any order, and gives you a chance to calculate stats for each placement and for each `GamePath`. 

A `GamePath` is essentially a list of up to 3 `Moves` that is calculate by the implementation. 

Deriving from this implementation, you will need to implement the `IFullEvalPlayer` interface

```csharp
interface IFullEvalPlayer
{
    GamePath SelectBestPath(List<GamePath> paths);
    void GatherStepStats(Candidate candidate, GamePath gamePath, Board board, Board newBoard);
    void GatherPathStats(GamePath gamePath, Board board);
}
```

## Testing

To test your implementation, edit the `Main()` method in `Game`, and instantiate your implementation of IPlayer in the `Game` constructor. 

```csharp
static void Main()
{
    var seed = DateTime.Now.Millisecond;

    var game = new Game(new FullEvalPlayer());    // Put here you IPlayer implementation
    game.rnd = new Random(seed);                  // Consider using a constant seed for debugging

    var start = DateTime.Now;
    game.Play();

    Console.WriteLine(@"Duration: {0:mm\:ss\.ff}", (DateTime.Now - start));
    Console.WriteLine("Game seed: " + seed);
    Console.ReadLine();
}
```
