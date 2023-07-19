﻿using LuaDecompilerCore.IR;

namespace LuaDecompilerCore.Passes;

public class RewriteVarargListAssignmentPass : IPass
{
    public void RunOnFunction(DecompilationContext context, Function f)
    {
        foreach (var b in f.BlockList)
        {
            for (int i = 0; i < b.Instructions.Count - 2; i++)
            {
                if (b.Instructions[i] is Assignment { Right: InitializerList l1 } a1 && l1.Expressions.Count == 0 &&
                    b.Instructions[i + 1] is Assignment { IsAmbiguousVararg: true } a2 && a1.VarargAssignmentReg == (a2.VarargAssignmentReg - 1) &&
                    b.Instructions[i + 2] is Assignment { IsAmbiguousVararg: true } a3 && a3.VarargAssignmentReg == a1.VarargAssignmentReg)
                {
                    l1.Expressions.Add(new Constant(Constant.ConstantType.ConstVarargs, -1));
                    a1.LocalAssignments = a3.LocalAssignments;
                    b.Instructions.RemoveRange(i + 1, 2);
                }
            }
        }
    }
}