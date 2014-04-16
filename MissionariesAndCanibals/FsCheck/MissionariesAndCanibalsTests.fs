namespace MissionariesAndCanibals

module MissionariesAndCanibalsTests =
    open System.Linq
    open Xunit
    open FsUnit.Xunit
    open MissionariesAndCanibals

    [<Fact>]
    let ``Initial state is allowed`` () =
        InitialState.IsStateAllowed() |>
            should be True

    [<Fact>]
    let ``Only canibals is allowed state`` () =
        (new State(Left, Canibals(1))).IsStateAllowed() |>
            should be True

    [<Fact>]
    let ``Only misionaries is allowed state`` () =
        (new State(Left, Missionaries(2))).IsStateAllowed() |>
            should be True

    [<Fact>]
    let ``More misionaries then canibals is allowed state`` () =
        (new State(Left, MissionariesAndCanibals(2, 1))).IsStateAllowed() |>
            should be True

    [<Fact>]
    let ``As many misionaries as canibals is allowed state`` () =
        (new State(Left, MissionariesAndCanibals(1, 1))).IsStateAllowed() |>
            should be True

    [<Fact>]
    let ``Less misionaries then canibals is not allowed state`` () =
        (new State(Left, MissionariesAndCanibals(1, 2))).IsStateAllowed() |>
            should be False

    [<Fact>]
    let ``Nobody on a bank is allowed state`` () =
        (new State(Left, Nobody)).IsStateAllowed() |>
            should be True

    [<Fact>]
    let ``There is nobody on right bank on initial state`` () =
        let expectedOppositeBank = new State(Right, Nobody)
        InitialState.OppositeBank() |>
            should equal expectedOppositeBank

    [<Fact>]
    let ``Opposite to bank with all missionaries is bank with all canibals`` () =
        let allMissionaries = new State(Right, Missionaries(3))
        let allCanibals = new State(Left, Canibals(3))

        allMissionaries.OppositeBank() |>
            should equal allCanibals

    [<Fact>]
    let ``Opposite to bank with all missionaries and two canibals is bank with one canibal`` () =
        let allMissionariesAndTwoCanicals = new State(Right, MissionariesAndCanibals(3, 2))
        let oneCanibal = new State(Left, Canibals(1))

        allMissionariesAndTwoCanicals.OppositeBank() |>
            should equal oneCanibal

    [<Fact>]
    let ``Opposite to bank with one canibal is bank with all missionaries and two canibals `` () =
        let oneCanibal = new State(Right, Canibals(1))
        let allMissionariesAndTwoCanicals = new State(Left, MissionariesAndCanibals(3, 2))

        oneCanibal.OppositeBank() |>
            should equal allMissionariesAndTwoCanicals

    [<Fact>]
    let ``There are three moved from initial state`` () =
        let possibleMoves = AllowedMoves(InitialState)

        possibleMoves.Count() |> should equal 3
        possibleMoves |> should contain (Transfer(Canibals(1)))
        possibleMoves |> should contain (Transfer(Canibals(2)))
        possibleMoves |> should contain (Transfer(MissionariesAndCanibals(1, 1)))

    [<Fact>]
    let ``There is no allowed moves from bank where there is nobody`` () =
        let possibleMoves = AllowedMoves(State(Left, Nobody))

        possibleMoves.Count() |> should equal 0

    [<Fact>]
    let ``There is only one allowed moves from bank where there is one canibal`` () =
        let possibleMoves = AllowedMoves(State(Left, Canibals(1)))

        possibleMoves.Count() |> should equal 1
        possibleMoves |> should contain (Transfer(Canibals(1)))

    [<Fact>]
    let ``There are three followed states from initial state`` () =
        let possibleMoves = FollowingStates(InitialState)

        possibleMoves.Count() |> should equal 3
        possibleMoves |> should contain (new State(Right, Canibals(1)))
        possibleMoves |> should contain (new State(Right, Canibals(2)))
        possibleMoves |> should contain (new State(Right, MissionariesAndCanibals(1, 1)))

    [<Fact>]
    let ``There is no allowed followed states from bank where there is nobody`` () =
        let possibleMoves = FollowingStates(State(Left, Nobody))

        possibleMoves.Count() |> should equal 0

    [<Fact>]
    let ``There is only one allowed followed state from bank where there is one canibal`` () =
        let possibleMoves = FollowingStates(State(Left, Canibals(1)))

        possibleMoves.Count() |> should equal 1
        possibleMoves |> should contain (new State(Right, MissionariesAndCanibals(3, 3)))

    [<Fact>]
    let ``State with everyone on right bank is goal state`` () =
        let everyoneOnRightBank = new State(Right, MissionariesAndCanibals(3, 3))

        IsGoalState(everyoneOnRightBank) |> should be True

    [<Fact>]
    let ``Initial state is not goal state`` () =
        IsGoalState(InitialState) |> should be False

    [<Fact>]
    let ``State with all misionaries and two canibals on right bank is not goal state`` () =
        let allMissionariesAndTwoCanibalsOnRightBank = new State(Right, MissionariesAndCanibals(3, 2))
        IsGoalState(allMissionariesAndTwoCanibalsOnRightBank) |> should be False

    open FSharpx.Collections

    [<Fact>]
    let ``Get element with smallest generation from PriorityQueue`` () =
        let first = CurrentState(0, InitialState, Start)
        let secondC1 = CurrentState(1, new State(Right, Canibals(1)), first)
        let secondC2 = CurrentState(1, new State(Right, Canibals(2)), first)
        let secondM1C1 = CurrentState(1, new State(Right, MissionariesAndCanibals(1, 1)), first)
        let thirdM3C2 = CurrentState(2, new State(Right, MissionariesAndCanibals(1, 1)), first)

        let priorityQueue = PriorityQueue.empty false
        let priorityQueue =  PriorityQueue.insert first priorityQueue
        let priorityQueue =  PriorityQueue.insert secondC1 priorityQueue
        let priorityQueue =  PriorityQueue.insert secondC2 priorityQueue
        let priorityQueue =  PriorityQueue.insert secondM1C1 priorityQueue
        let priorityQueue =  PriorityQueue.insert thirdM3C2 priorityQueue

        let (nextElement, priorityQueue) = PriorityQueue.pop priorityQueue
        nextElement |> should equal first

        let (nextElement, priorityQueue) = PriorityQueue.pop priorityQueue
        nextElement |> should equal secondM1C1

        let (nextElement, priorityQueue) = PriorityQueue.pop priorityQueue
        nextElement |> should equal secondC2

        let (nextElement, priorityQueue) = PriorityQueue.pop priorityQueue
        nextElement |> should equal secondC1

        let (nextElement, priorityQueue) = PriorityQueue.pop priorityQueue
        nextElement |> should equal thirdM3C2

        PriorityQueue.isEmpty priorityQueue |> should be True

    [<Fact>]
    let ``Get goal state after processing from initial state`` () =
        let goal = Run()

        match goal with
        | CurrentState(_, goalState, _) ->
            let expectedGoalState = new State(Right, MissionariesAndCanibals(3, 3))

            goalState |> should equal expectedGoalState
        | _ -> "this should not be" |> should be EmptyString

    [<Fact>]
    let ``Get goal state after 11 steps`` () =
        let goal = Run()

        match goal with
        | CurrentState(11, _, _) -> ()
        | _ -> "this should not be" |> should be EmptyString