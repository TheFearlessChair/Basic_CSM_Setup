module TypesAST

type AExpr =
  | Num                  of  (int)
  | Var                  of  (string)
  | Mult                 of  (AExpr * AExpr)
  | Div                  of  (AExpr * AExpr)
  | Plus                 of  (AExpr * AExpr)
  | Minus                of  (AExpr * AExpr)
  | Pow                  of  (AExpr * AExpr)
  | UPlus                of  (AExpr)
  | UMinus               of  (AExpr)

and BExpr =
  | Bool                 of  (bool)

and Command =
  | Skip

and GuardedCommand = 
  | Eval                 of  (BExpr * Command)