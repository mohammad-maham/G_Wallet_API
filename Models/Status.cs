﻿using System;
using System.Collections.Generic;

namespace G_Wallet_API.Models;

public partial class Status
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public string Caption { get; set; } = null!;
}
