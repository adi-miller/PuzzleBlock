# The Puzzle Block Challenge
Welcome to the Puzzle Block Challenge. This challenge is based on the [Block Puzzle](https://play.google.com/store/apps/details?id=com.differencetenderwhite.skirt) game. 

The purpose of this challenge is to code a new implementation to the `IPlayer` interface, which will beat all other implementation by gaining the highest score. 

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
The implementation receives a `board` object, as well as a dictionary of `shapes`, and needs to output the chosen `shapeId` to place and a `placement` string descriptor that specifies where to place the `shape`. 

The method receives the following parameters:

* `out int shapeId`: The implementation must output a value between 1 and 3 to the caller, which will indicate which shape needs to be placed in this turn. Use the `key` from the `shapes` dictionary to determine the `shapeId`
* `out string placement`: The implementaiton must output a 2 char string indicating the location to place the shape. The first char should be alphanumeric 'a'-'h' and the second char should be numberic '1'-'8'. For example, return `b4` to place the shape starting from the upper-left corner in b4.
* `Board board`: The `board` object includes all the information about the current state of the `board`, as well as different methods to test and manipulate the board. 
* `IDictionary<int, Shape> shapes`: A dictionary of the currently available shapes, that needs to be placed in this turn. The dictionary maps between the `int shapeId` and the actual `shape` object.
* `IGameDrawer renderer`: Can be used to render output to the Console using the `ConsoleGameDrawer` implementation

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
