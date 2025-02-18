namespace GroovyParserBackend.Entities
{
    public enum TokenType
    {
        None,

        NumberLiteral, // [DONE]
        StringLiteral, // [DONE]

        Identifier,     // [DONE]
        Keyword,        // [DONE]

        FunctionCall, // [DONE]

        TypeAnnotation, // isn't needed

        Parentheses,    // [DONE]
        Brackets,       // [DONE]
        Braces,         // [DONE]

        Equal,          // [DONE]
        GreaterThan,    // [DONE]
        LessThan,       // [DONE]
        GreaterOrEqual, // [DONE]
        LessOrEqual,    // [DONE]
        NotEqual,       // [DONE]
        Identical,      // [DONE]
        NotIdentical,   // [DONE]

        TernaryOperator,

        ElvisOperator,  // [DONE] 
        ElvisAssignment,// [DONE] 

        BitwiseAnd,     // [DONE]
        BitwiseOr,      // [DONE]
        BitwiseNot,     // [DONE]
        BitwiseXor,     // [DONE]
        LeftShift,      // [DONE] 
        RightShift,     // [DONE] 
        UnsignedRightShift, // [DONE] 

        And,    // [DONE]
        Or,     // [DONE]
        Not,    // [DONE]

        Assignment, // [DONE]
        Plus, // [DONE]
        Minus, // [DONE]
        Star,   // [DONE]
        Slash, // [DONE]
        Percent, //[DONE]
        DoubleStar, // [DONE]

        PlusAssignment, // [DONE]
        MinusAssignment, // [DONE]
        StarAssignment, // [DONE]
        SlashAssignment, // [DONE]
        PercentAssignment, // [DONE]
        DoubleStarAssignment, //[DONE]

        UnaryPlus, // [DONE]
        UnaryMinus, // [DONE]

        PrefixIncrement, // [DONE]
        PrefixDecrement, // [DONE]
        PostfixIncrement, // [DONE]
        PostfixDecrement, // [DONE]

        MemberAccess,           // [DONE]
        NullSafeMemberAccess,   // [DONE]
        MethodPointer,          // ?????????????????  sth.&method

        PatternOperator,    // [DONE]
        FindOperator,       // [DONE]
        MatchOperator,      // [DONE]

        SpreadOperator,     // [DONE]

        RangeOperator, // 1..5 or 'a'..'z'... damn... 

        SpaceshipOperator,  // [DONE]

        SubscriptOperator,     // [DONE]
        SafeSubscriptOperator, // [DONE]

        MembershipOperator, // [DONE]

        CoercionOperation,  // [DONE]

        DiamondOperator, // ???????????????????????
    }
}
