var nTimes : int := 0;
print "How many times?"; 
read nTimes; 
var x : int;
for x in 0..nTimes-1 do 
   print x;
   print "   - Hello, World!";
end for;
assert (x = nTimes);
