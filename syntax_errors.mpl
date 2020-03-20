/*

Test: syntactix errors

*/


var no_type := 1;   // Missing type
var bad_initial_value : int = 1;   // Wrong assignment symbol

var j: int;
for j in 1 . 5 do   // single dot
   print true;
end for;

for j in 1 .. 5    // missing do
   print false;
end for;

for j in 1 . 5    // missing dot AND do
   nonsense .. := int;  // all contents of the loop skipped in this case
end for;

for j in 1 .. 2 do
   print j;
end;    // missing for

var v : int;
v : (3 * 99);   // wrong assignment symbol
v;              // missing assignment
v := 42;

print (((j - 5) / 3 nonsense! here) * 10);  // Error inside a nested expression
print (((j - 5) / 3 nonsense here * 10);  // Error inside a nested expression
