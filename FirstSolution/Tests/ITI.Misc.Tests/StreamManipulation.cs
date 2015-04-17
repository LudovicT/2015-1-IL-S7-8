﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

                                                                                                                                          

namespace ITI.Misc.Tests
{
    [TestFixture]
    public class StreamManipulation
    {
        const string fileToRead = @"E:\Intech\2015-1\S7-8\Dev\2015-1-IL-S7-8\FirstSolution\Tests\ITI.Misc.Tests\StreamManipulation.cs"; 

        [Test]
        public void reading_a_file()
        {
            var bytes = Encoding.Unicode.GetBytes( "BonالموJMam" );
            var bytesUTF8 = Encoding.UTF8.GetBytes( "BonالموJMam" ); 

            using( var s = new FileStream( fileToRead, FileMode.Open, FileAccess.Read, FileShare.None ) )
            {
                Assert.That( s.CanRead );
                Assert.That( s.CanWrite, Is.False );
                byte[] buffer = new byte[64];
                s.Read( buffer, 0, buffer.Length );
            }
        }

        [Test]
        public void reading_and_writing_a_file()
        {
            byte[] buffer = new byte[12];
            using( var input = new FileStream( fileToRead, FileMode.Open, FileAccess.Read, FileShare.None ) )
            using( var output = new FileStream( fileToRead + ".copy", FileMode.Create, FileAccess.Write, FileShare.None ) )
            {
                int lenRead;
                while( (lenRead = input.Read( buffer, 0, buffer.Length )) > 0 )
                {
                    output.Write( buffer, 0, lenRead );
                }
            }
            FileAssert.AreEqual( fileToRead, fileToRead + ".copy" );
        }

        [Test]
        public void reading_and_writing_a_fucked_file()
        {
            byte[] buffer = new byte[12];
            using( var input = new FileStream( fileToRead, FileMode.Open, FileAccess.Read, FileShare.None ) )
            using( var output = new FileStream( fileToRead + ".ccopy", FileMode.Create, FileAccess.Write, FileShare.None ) )
            {
                int lenRead;
                while( (lenRead = input.Read( buffer, 0, buffer.Length )) > 0 )
                {
                    for( int i = 0; i < lenRead; ++i )
                    {
                        buffer[i] ^= 15;
                    }
                    output.Write( buffer, 0, lenRead );
                }
            }
            using( var input = new FileStream( fileToRead + ".ccopy", FileMode.Open, FileAccess.Read, FileShare.None ) )
            using( var output = new FileStream( fileToRead+ ".decopy", FileMode.Create, FileAccess.Write, FileShare.None ) )
            {
                int lenRead;
                while( (lenRead = input.Read( buffer, 0, buffer.Length )) > 0 )
                {
                    for( int i = 0; i < lenRead; ++i )
                    {
                        buffer[i] ^= 15;
                    }
                    output.Write( buffer, 0, lenRead );
                }
            }
            FileAssert.AreNotEqual( fileToRead, fileToRead + ".ccopy" );
            FileAssert.AreEqual( fileToRead, fileToRead + ".decopy" );
        }


        [Test]
        public void krabouille_and_dekrabouille()
        {
            using( var input = new FileStream( fileToRead, FileMode.Open, FileAccess.Read, FileShare.None ) )
            using( var output = new FileStream( fileToRead + ".krab", FileMode.Create, FileAccess.Write, FileShare.None ) )
            using( var krab = new KrabouilleStream( output, KrabouilleMode.Krabouille, "Super Mot de Passe" ) )
            {
                input.CopyTo( krab );
            }
            using( var fileInput = new FileStream( fileToRead + ".krab", FileMode.Open, FileAccess.Read, FileShare.None ) )
            using( var input = new KrabouilleStream( fileInput, KrabouilleMode.Dekrabouille, "Super Mot de Passe" ) )
            using( var output = new FileStream( fileToRead + ".clear", FileMode.Create, FileAccess.Write, FileShare.None ) )
            {
                input.CopyTo( output );
            }
            FileAssert.AreNotEqual( fileToRead, fileToRead + ".krab" );
            FileAssert.AreEqual( fileToRead, fileToRead + ".clear" );
        }

    }
}
