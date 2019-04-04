using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace formelemethoden
{
    class ThompsonBox
    {
        public RegExp Exp { get; set; }
        public string BeginState  { get; set; }
        public string EndState { get; set; }

        public ThompsonBox(RegExp exp, string beginState, string endState)
        {
            this.Exp = exp;
            this.BeginState = beginState;
            this.EndState = endState;
        }

        public List<Transition<string>> GetTransitions()
        {
            return Exp.GetTransitions(BeginState, EndState);
        }
    }
}
