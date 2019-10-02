﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luadec.IR
{
    public class Jump : IInstruction
    {
        public Label Dest;
        // For debug pretty printing only
        public CFG.BasicBlock BBDest = null;
        public bool Conditional;
        public Expression Condition;

        public Jump(Label dest)
        {
            Dest = dest;
            Conditional = false;
        }

        public Jump(Label dest, Expression cond)
        {
            Dest = dest;
            Conditional = true;
            Condition = cond;
            if (Condition is BinOp op)
            {
                op.NegateCondition();
            }
        }

        public override HashSet<Identifier> GetUses(bool regonly)
        {
            if (Conditional)
            {
                return Condition.GetUses(regonly);
            }
            return base.GetUses(regonly);
        }

        public override void RenameUses(Identifier orig, Identifier newi)
        {
            if (Conditional)
            {
                Condition.RenameUses(orig, newi);
            }
        }

        public override bool ReplaceUses(Identifier orig, Expression sub)
        {
            if (Conditional)
            {
                if (Expression.ShouldReplace(orig, Condition))
                {
                    Condition = sub;
                    return true;
                }
                else
                {
                    return Condition.ReplaceUses(orig, sub);
                }
            }
            return false;
        }

        public override string ToString()
        {
            string ret = "";
            if (Conditional)
            {
                ret += $@"if {Condition} else ";
            }
            if (BBDest != null)
            {
                ret += "goto " + BBDest;
            }
            else
            {
                ret += "goto " + Dest;
            }
            return ret;
        }
    }
}
