#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2014 SoapBox Automation, All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Snap.
/// 
/// SoapBox Snap is free software: you can redistribute it and/or modify it
/// under the terms of the GNU General Public License as published by the 
/// Free Software Foundation, either version 3 of the License, or 
/// (at your option) any later version.
/// 
/// SoapBox Snap is distributed in the hope that it will be useful, but 
/// WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with SoapBox Snap. If not, see <http://www.gnu.org/licenses/>.
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoapBox.Snap.Twitter
{
    /// <summary>
    /// Ok, so in order for an application to use the Twitter API, you have to sign up for 
    /// a Twitter consumer key and consumer secret.  This just means you're registering your
    /// application with Twitter.  You can do that here: http://dev.twitter.com/apps/new
    /// 
    /// Once you do that, they'll give you a Consumer Key and a Consumer Secret, which are just
    /// strings.  These are used to authenticate your application when you use the API, either
    /// with OAuth or xAuth.  Now OAuth is great for web applications, and in that case your
    /// source code and binary is sitting (presumably) safely on a web server somewhere, so
    /// you're never exposing your key or secret to the world.  For desktop applications, like
    /// SoapBox Snap, you have to embed these into the application.
    /// 
    /// I chose to use xAuth for SoapBox Snap to make it simpler for the user.  That means I don't
    /// have to send the user to a new web page to sign in with their username and password... they
    /// just enter it into SoapBox Snap and we take care of everything.  However, that means I had
    /// to send an email to api@twitter.com to get them to enable xAuth for this application.
    /// Note that if you replace these keys with your own, you will need to do the same.
    /// 
    /// During that email exchange, they said that my application key and secret need to be
    /// kept hidden.  They did acknowledge the near-impossibility of this request, of course, 
    /// because (a) it's an open source application, and (b) even with a binary, you could
    /// decompile it and get the strings out of it.  Twitter asked for a "best faith effort"
    /// or something like that.  So here it is... absolutely pathetically stupid Rot13 encoding
    /// of the key and secret.  All this really does is protect the strings from a naive
    /// dumping of the strings in the binaries and they're not distributed in "plain text"
    /// in the source code.
    /// 
    /// Ultimately I lay this squarely on the shoulders of Twitter.  If there's a better way,
    /// I'm willing to listen, but I'm not wasting more of my time thinking about it.
    /// 
    /// FYI, to replace these strings with new ones, use http://www.rot13.com/
    /// </summary>
    class TwitterConsumer
    {

        public static string ConsumerKey
        {
            get
            {
                return Rot13.Transform("UwQJ4H7SjuDMG3SyGGE5wD");
            }
        }

        public static string ConsumerSecret
        {
            get
            {
                return Rot13.Transform("daosxQsJoPStA8SAoyZclbNwxLKEhcu9VEZRH79lBZ");
            }
        }
    }
}
