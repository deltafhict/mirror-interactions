// ***********************************************************************
// Assembly         : MirrorInteractions
// Author           : daan
// Created          : 06-24-2015
//
// Last Modified By : daan
// Last Modified On : 06-24-2015
// ***********************************************************************
// <copyright file="SpeechDelegate.cs" company="Delta">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>Class used for communication delegates.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Speech namespace, all Speech related classes are in this namespace.
/// </summary>
namespace MirrorInteractions.Speech
{
    /// <summary>
    /// Class used for communication delegates.
    /// </summary>
    public abstract class SpeechDelegate
    {
        /// <summary>
        /// Delegate SpeechCalibratedDelegate
        /// </summary>
        /// <param name="threshold">The threshold.</param>
        public delegate void SpeechCalibratedDelegate(double threshold);
        /// <summary>
        /// Delegate SpeechCalibrateDelegate
        /// </summary>
        public delegate void SpeechCalibrateDelegate();
    }
}
