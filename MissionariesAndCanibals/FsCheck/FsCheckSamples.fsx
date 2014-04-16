#I @".\packages\FsCheck.0.9.2.0\lib\net40-Client\"
#r @"FsCheck.dll"

// #time "on"

open FsCheck

// simple example
let revRevIsOrig (xs:list<int>) = List.rev(List.rev xs) = xs
Check.Quick revRevIsOrig;;

// unstable example
let revIsSortedOrig (xs:list<int>) = List.rev xs = List.sort xs
Check.Quick revIsSortedOrig;;

// display the counter-example
Check.Verbose revIsSortedOrig;;

// nan is one of corner cases for float type: nan <> nan
let revRevIsOrigFloat (xs:list<float>) = List.rev(List.rev xs) = xs
Check.Quick revRevIsOrigFloat;;

// grouping properties
type ListProperties =
    static member ``reverse of reverse is original`` xs = revRevIsOrig xs
    static member ``reverse is sorted original`` xs = revIsSortedOrig xs

Check.QuickAll<ListProperties>();;

// Conditional Properties (may take the form ==>)

let rec ordered xs = 
   match xs with
   | [] -> true
   | [x] -> true
   | x::y::ys ->  (x <= y) && ordered (y::ys)

let rec insert x xs = 
   match xs with
   | [] -> [x]
   | c::cs -> if x <= c then x::xs else c::(insert x cs)

let Insert (x:int) xs = ordered xs ==> ordered (insert x xs)

Check.Quick Insert;;
// Check.Verbose Insert;;

// counting trivial and classifying Test Cases
let trivialInsert (x:int) xs = ordered xs ==> ordered (insert x xs)
                                |> Prop.trivial (List.length xs = 0)
                                |> Prop.classify (ordered (x::xs)) "at-head"
                                |> Prop.classify (ordered (xs @ [x])) "at-tail" 
                                |> Prop.collect (List.length xs)

Check.Quick trivialInsert;;

//#time "off"