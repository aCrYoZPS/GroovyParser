namespace GroovyParserBackend.Entities
{
    public enum TokenType
    {
        None,

        NumberLiteral,
        StringLiteral,

        Identifier,
        Keyword,        // [DONE]

        FunctionCall, // () )))

        TypeAnnotation,

        Parentheses,
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

        Assignment,
        Plus, // [DONE]
        Minus, // [DONE]
        Star,
        Slash,
        Percent,
        DoubleStar,

        PlusAssignment, // [DONE]
        MinusAssignment, // [DONE]
        StarAssignment,
        SlashAssignment,
        PercentAssignment,
        DoubleStarAssignment,

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
