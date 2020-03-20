
/*

Test:

   /* 0. 
      /* nested */ 
      comments 
   */

 1. nested for loops
 2. variable declaration inside for loop
 3. value of loop variables after the loop

*/


// Print numbers from 0 to p^3 - 1 using three nested for loops,
// and some simple arithmetic.

var i : int;
var j : int;
var k : int;
var p : int := 4;

for i in 0 .. p - 1 do
   for j in 0 .. p - 1 do
      for k in 0 .. p - 1 do
         var n : int := ((p*p) * i) + ((p * j) + k);
         print n;
      end for;
   end for;
end for;

assert (n = ((p*(p*p)) - 1));
assert ((i = p) & ((j = p) & (k = p)));
