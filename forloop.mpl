print "Demonstration of declaring a variable inside for loop.";
var i : int;

for i in 1 .. 5 do
   var j : int;
   assert (j = 0);   // Default initial value
   j := 10 * i;
   print j;
end for;

print "After the loop, the value of j is:";
print j;
