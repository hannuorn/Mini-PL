/*

Test:

 1. Invalid characters in source code
 2. Nonterminated string literals

*/


var ördäys : string := "Hurjaa menoa";

var nonterminated1 : string := "Oops;
;
var nonterminated2 : string := "Backslash messing up termination\"fff\"
;
var terminated : string := "This is properly terminated despite the backslash.\\";
