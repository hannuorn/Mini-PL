(*

Test: semantic errors

*)


// This source file produces all possible errors detected by the semantic analyser.


// declaration: variable not yet declared

var intvar : int := 1;
var intvar : int;

// assignment: declared

unknown_var := 2;

// assignment: type is a match

intvar := "a";
var stringvar : string;
stringvar := 3;
var boole : bool;
boole := 1;
intvar := boole;

// assignment: not a for-loop-variable

for intvar in 1 .. 3 do
   intvar := 4;  // not allowed
end for;
intvar := 5;  // allowed

// for_statement: declared

for undeclared in 1 .. 2 do
   print "foo";
end for;

// for_statement: is int

var boolvar : bool;
for boolvar in 1 .. 2 do
   print "bar";
end for;

// for_statement: not already a loop variable

var innervar : int;
for intvar in 1 .. 3 do
   for innervar in 10 .. 11 do
      for intvar in 100 .. 111 do
         print "hi";
      end for;
   end for;
end for;

// for_statement: boundaries are int

var y : int;
for y in "a" .. true do
   print "y";
end for;

// read_statement: variable is int or string

read boolvar;

// print_statement: int or string

print boolvar;

// assert_statement: expression is bool

assert (intvar); // int
assert (stringvar); // string

// binary_operator: type match

print intvar + stringvar;

// plus: int or string

print false + true;

// minus: int

print false - true;
print "a" - "b";

// *: int

print false * true;
print "a" * "b";

// division: int

print false / true;
print "a" / "b";

// equal and less accept any type

// &: bool

print 1 & 2;
print "a" & "b";

// unary_operator: bool

print !intvar;
print !stringvar;

// variable reference: must be declared

print undeclared;
