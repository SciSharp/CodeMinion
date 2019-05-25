using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Python.Runtime;
using Python.Included;
using Numpy.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using LA = Numpy.np.linalg;

namespace Numpy.UnitTest
{
    [TestClass]
    public class NumPy_linalg_fftTest : BaseTestCase
    {
        [TestMethod]
        public void choleskyTest()
        {
            // >>> A = np.array([[1,-2j],[2j,5]])
            // >>> A
            // array([[ 1.+0.j,  0.-2.j],
            //        [ 0.+2.j,  5.+0.j]])
            // >>> L = np.linalg.cholesky(A)
            // >>> L
            // array([[ 1.+0.j,  0.+0.j],
            //        [ 0.+2.j,  1.+0.j]])
            // >>> np.dot(L, L.T.conj()) # verify that L * L.H = A
            // array([[ 1.+0.j,  0.-2.j],
            //        [ 0.+2.j,  5.+0.j]])
            // >>> A = [[1,-2j],[2j,5]] # what happens if A is only array_like?
            // >>> np.linalg.cholesky(A) # an ndarray object is returned
            // array([[ 1.+0.j,  0.+0.j],
            //        [ 0.+2.j,  1.+0.j]])
            // >>> # But a matrix object is returned if A is a matrix object
            // >>> LA.cholesky(np.matrix(A))
            // matrix([[ 1.+0.j,  0.+0.j],
            //         [ 0.+2.j,  1.+0.j]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  A = np.array([[1,-2j],[2j,5]]);
            given=  A;
            expected=
                "array([[ 1.+0.j,  0.-2.j],\n" +
                "       [ 0.+2.j,  5.+0.j]])";
            Assert.AreEqual(expected, given.repr);
            given=  L = np.linalg.cholesky(A);
            given=  L;
            expected=
                "array([[ 1.+0.j,  0.+0.j],\n" +
                "       [ 0.+2.j,  1.+0.j]])";
            Assert.AreEqual(expected, given.repr);
            given=  np.dot(L, L.T.conj()) # verify that L * L.H = A;
            expected=
                "array([[ 1.+0.j,  0.-2.j],\n" +
                "       [ 0.+2.j,  5.+0.j]])";
            Assert.AreEqual(expected, given.repr);
            given=  A = [[1,-2j],[2j,5]] # what happens if A is only array_like?;
            given=  np.linalg.cholesky(A) # an ndarray object is returned;
            expected=
                "array([[ 1.+0.j,  0.+0.j],\n" +
                "       [ 0.+2.j,  1.+0.j]])";
            Assert.AreEqual(expected, given.repr);
            given=  # But a matrix object is returned if A is a matrix object;
            given=  LA.cholesky(np.matrix(A));
            expected=
                "matrix([[ 1.+0.j,  0.+0.j],\n" +
                "        [ 0.+2.j,  1.+0.j]])";
            Assert.AreEqual(expected, given.repr);
            #endif
        }

        [TestMethod]
        public void detTest()
        {
            // The determinant of a 2-D array [[a, b], [c, d]] is ad - bc:
            
            // >>> a = np.array([[1, 2], [3, 4]])
            // >>> np.linalg.det(a)
            // -2.0
            // 

            var  a = np.array(new int[,] {{1, 2}, {3, 4}});
            var given=  np.linalg.det(a);
            var expected=
                "-2.0";
            Assert.AreEqual(expected, given.repr.Substring(0, 4));
            
            // Computing determinants for a stack of matrices:
            
            // >>> a = np.array([ [[1, 2], [3, 4]], [[1, 2], [2, 1]], [[1, 3], [3, 1]] ])
            // >>> a.shape
            // (3, 2, 2)
            // >>> np.linalg.det(a)
            // array([-2., -3., -8.])
            // 

              a = np.array(new int[,,] { { {1, 2}, {3, 4}}, {{1, 2}, {2, 1}}, {{1, 3}, {3, 1}} });
            var shape=  a.shape;
            expected=
                "(3, 2, 2)";
            Assert.AreEqual(expected, shape.ToString());
            given=  np.linalg.det(a);
            expected=
                "array([-2., -3., -8.])";
            Assert.AreEqual(expected, given.repr);
        }

        [TestMethod]
        public void eigTest()
        {
            // >>> from numpy import linalg as LA
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  from numpy import linalg as LA;
            #endif
            // (Almost) trivial example with real e-values and e-vectors.
            
            // >>> w, v = LA.eig(np.diag((1, 2, 3)))
            // >>> w; v
            // array([ 1.,  2.,  3.])
            // array([[ 1.,  0.,  0.],
            //        [ 0.,  1.,  0.],
            //        [ 0.,  0.,  1.]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  w, v = LA.eig(np.diag((1, 2, 3)));
            given=  w; v;
            expected=
                "array([ 1.,  2.,  3.])\n" +
                "array([[ 1.,  0.,  0.],\n" +
                "       [ 0.,  1.,  0.],\n" +
                "       [ 0.,  0.,  1.]])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Real matrix possessing complex e-values and e-vectors; note that the
            // e-values are complex conjugates of each other.
            
            // >>> w, v = LA.eig(np.array([[1, -1], [1, 1]]))
            // >>> w; v
            // array([ 1. + 1.j,  1. - 1.j])
            // array([[ 0.70710678+0.j        ,  0.70710678+0.j        ],
            //        [ 0.00000000-0.70710678j,  0.00000000+0.70710678j]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  w, v = LA.eig(np.array([[1, -1], [1, 1]]));
            given=  w; v;
            expected=
                "array([ 1. + 1.j,  1. - 1.j])\n" +
                "array([[ 0.70710678+0.j        ,  0.70710678+0.j        ],\n" +
                "       [ 0.00000000-0.70710678j,  0.00000000+0.70710678j]])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Complex-valued matrix with real e-values (but complex-valued e-vectors);
            // note that a.conj().T = a, i.e., a is Hermitian.
            
            // >>> a = np.array([[1, 1j], [-1j, 1]])
            // >>> w, v = LA.eig(a)
            // >>> w; v
            // array([  2.00000000e+00+0.j,   5.98651912e-36+0.j]) # i.e., {2, 0}
            // array([[ 0.00000000+0.70710678j,  0.70710678+0.j        ],
            //        [ 0.70710678+0.j        ,  0.00000000+0.70710678j]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  a = np.array([[1, 1j], [-1j, 1]]);
            given=  w, v = LA.eig(a);
            given=  w; v;
            expected=
                "array([  2.00000000e+00+0.j,   5.98651912e-36+0.j]) # i.e., {2, 0}\n" +
                "array([[ 0.00000000+0.70710678j,  0.70710678+0.j        ],\n" +
                "       [ 0.70710678+0.j        ,  0.00000000+0.70710678j]])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Be careful about round-off error!
            
            // >>> a = np.array([[1 + 1e-9, 0], [0, 1 - 1e-9]])
            // >>> # Theor. e-values are 1 +/- 1e-9
            // >>> w, v = LA.eig(a)
            // >>> w; v
            // array([ 1.,  1.])
            // array([[ 1.,  0.],
            //        [ 0.,  1.]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  a = np.array([[1 + 1e-9, 0], [0, 1 - 1e-9]]);
            given=  # Theor. e-values are 1 +/- 1e-9;
            given=  w, v = LA.eig(a);
            given=  w; v;
            expected=
                "array([ 1.,  1.])\n" +
                "array([[ 1.,  0.],\n" +
                "       [ 0.,  1.]])";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        [TestMethod]
        public void eighTest()
        {
            // >>> from numpy import linalg as LA
            // >>> a = np.array([[1, -2j], [2j, 5]])
            // >>> a
            // array([[ 1.+0.j,  0.-2.j],
            //        [ 0.+2.j,  5.+0.j]])
            // >>> w, v = LA.eigh(a)
            // >>> w; v
            // array([ 0.17157288,  5.82842712])
            // array([[-0.92387953+0.j        , -0.38268343+0.j        ],
            //        [ 0.00000000+0.38268343j,  0.00000000-0.92387953j]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  from numpy import linalg as LA;
            given=  a = np.array([[1, -2j], [2j, 5]]);
            given=  a;
            expected=
                "array([[ 1.+0.j,  0.-2.j],\n" +
                "       [ 0.+2.j,  5.+0.j]])";
            Assert.AreEqual(expected, given.repr);
            given=  w, v = LA.eigh(a);
            given=  w; v;
            expected=
                "array([ 0.17157288,  5.82842712])\n" +
                "array([[-0.92387953+0.j        , -0.38268343+0.j        ],\n" +
                "       [ 0.00000000+0.38268343j,  0.00000000-0.92387953j]])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // >>> np.dot(a, v[:, 0]) - w[0] * v[:, 0] # verify 1st e-val/vec pair
            // array([2.77555756e-17 + 0.j, 0. + 1.38777878e-16j])
            // >>> np.dot(a, v[:, 1]) - w[1] * v[:, 1] # verify 2nd e-val/vec pair
            // array([ 0.+0.j,  0.+0.j])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  np.dot(a, v[:, 0]) - w[0] * v[:, 0] # verify 1st e-val/vec pair;
            expected=
                "array([2.77555756e-17 + 0.j, 0. + 1.38777878e-16j])";
            Assert.AreEqual(expected, given.repr);
            given=  np.dot(a, v[:, 1]) - w[1] * v[:, 1] # verify 2nd e-val/vec pair;
            expected=
                "array([ 0.+0.j,  0.+0.j])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // >>> A = np.matrix(a) # what happens if input is a matrix object
            // >>> A
            // matrix([[ 1.+0.j,  0.-2.j],
            //         [ 0.+2.j,  5.+0.j]])
            // >>> w, v = LA.eigh(A)
            // >>> w; v
            // array([ 0.17157288,  5.82842712])
            // matrix([[-0.92387953+0.j        , -0.38268343+0.j        ],
            //         [ 0.00000000+0.38268343j,  0.00000000-0.92387953j]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  A = np.matrix(a) # what happens if input is a matrix object;
            given=  A;
            expected=
                "matrix([[ 1.+0.j,  0.-2.j],\n" +
                "        [ 0.+2.j,  5.+0.j]])";
            Assert.AreEqual(expected, given.repr);
            given=  w, v = LA.eigh(A);
            given=  w; v;
            expected=
                "array([ 0.17157288,  5.82842712])\n" +
                "matrix([[-0.92387953+0.j        , -0.38268343+0.j        ],\n" +
                "        [ 0.00000000+0.38268343j,  0.00000000-0.92387953j]])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // >>> # demonstrate the treatment of the imaginary part of the diagonal
            // >>> a = np.array([[5+2j, 9-2j], [0+2j, 2-1j]])
            // >>> a
            // array([[ 5.+2.j,  9.-2.j],
            //        [ 0.+2.j,  2.-1.j]])
            // >>> # with UPLO='L' this is numerically equivalent to using LA.eig() with:
            // >>> b = np.array([[5.+0.j, 0.-2.j], [0.+2.j, 2.-0.j]])
            // >>> b
            // array([[ 5.+0.j,  0.-2.j],
            //        [ 0.+2.j,  2.+0.j]])
            // >>> wa, va = LA.eigh(a)
            // >>> wb, vb = LA.eig(b)
            // >>> wa; wb
            // array([ 1.,  6.])
            // array([ 6.+0.j,  1.+0.j])
            // >>> va; vb
            // array([[-0.44721360-0.j        , -0.89442719+0.j        ],
            //        [ 0.00000000+0.89442719j,  0.00000000-0.4472136j ]])
            // array([[ 0.89442719+0.j       ,  0.00000000-0.4472136j],
            //        [ 0.00000000-0.4472136j,  0.89442719+0.j       ]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  # demonstrate the treatment of the imaginary part of the diagonal;
            given=  a = np.array([[5+2j, 9-2j], [0+2j, 2-1j]]);
            given=  a;
            expected=
                "array([[ 5.+2.j,  9.-2.j],\n" +
                "       [ 0.+2.j,  2.-1.j]])";
            Assert.AreEqual(expected, given.repr);
            given=  # with UPLO='L' this is numerically equivalent to using LA.eig() with:;
            given=  b = np.array([[5.+0.j, 0.-2.j], [0.+2.j, 2.-0.j]]);
            given=  b;
            expected=
                "array([[ 5.+0.j,  0.-2.j],\n" +
                "       [ 0.+2.j,  2.+0.j]])";
            Assert.AreEqual(expected, given.repr);
            given=  wa, va = LA.eigh(a);
            given=  wb, vb = LA.eig(b);
            given=  wa; wb;
            expected=
                "array([ 1.,  6.])\n" +
                "array([ 6.+0.j,  1.+0.j])";
            Assert.AreEqual(expected, given.repr);
            given=  va; vb;
            expected=
                "array([[-0.44721360-0.j        , -0.89442719+0.j        ],\n" +
                "       [ 0.00000000+0.89442719j,  0.00000000-0.4472136j ]])\n" +
                "array([[ 0.89442719+0.j       ,  0.00000000-0.4472136j],\n" +
                "       [ 0.00000000-0.4472136j,  0.89442719+0.j       ]])";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        [TestMethod]
        public void eigvalsTest()
        {
            // Illustration, using the fact that the eigenvalues of a diagonal matrix
            // are its diagonal elements, that multiplying a matrix on the left
            // by an orthogonal matrix, Q, and on the right by Q.T (the transpose
            // of Q), preserves the eigenvalues of the “middle” matrix.  In other words,
            // if Q is orthogonal, then Q * A * Q.T has the same eigenvalues as
            // A:
            
            // >>> from numpy import linalg as LA
            // >>> x = np.random.random()
            // >>> Q = np.array([[np.cos(x), -np.sin(x)], [np.sin(x), np.cos(x)]])
            // >>> LA.norm(Q[0, :]), LA.norm(Q[1, :]), np.dot(Q[0, :],Q[1, :])
            // (1.0, 1.0, 0.0)
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  from numpy import linalg as LA;
            given=  x = np.random.random();
            given=  Q = np.array([[np.cos(x), -np.sin(x)], [np.sin(x), np.cos(x)]]);
            given=  LA.norm(Q[0, :]), LA.norm(Q[1, :]), np.dot(Q[0, :],Q[1, :]);
            expected=
                "(1.0, 1.0, 0.0)";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Now multiply a diagonal matrix by Q on one side and by Q.T on the other:
            
            // >>> D = np.diag((-1,1))
            // >>> LA.eigvals(D)
            // array([-1.,  1.])
            // >>> A = np.dot(Q, D)
            // >>> A = np.dot(A, Q.T)
            // >>> LA.eigvals(A)
            // array([ 1., -1.])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  D = np.diag((-1,1));
            given=  LA.eigvals(D);
            expected=
                "array([-1.,  1.])";
            Assert.AreEqual(expected, given.repr);
            given=  A = np.dot(Q, D);
            given=  A = np.dot(A, Q.T);
            given=  LA.eigvals(A);
            expected=
                "array([ 1., -1.])";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        [TestMethod]
        public void eigvalshTest()
        {
            // >>> from numpy import linalg as LA
            // >>> a = np.array([[1, -2j], [2j, 5]])
            // >>> LA.eigvalsh(a)
            // array([ 0.17157288,  5.82842712])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  from numpy import linalg as LA;
            given=  a = np.array([[1, -2j], [2j, 5]]);
            given=  LA.eigvalsh(a);
            expected=
                "array([ 0.17157288,  5.82842712])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // >>> # demonstrate the treatment of the imaginary part of the diagonal
            // >>> a = np.array([[5+2j, 9-2j], [0+2j, 2-1j]])
            // >>> a
            // array([[ 5.+2.j,  9.-2.j],
            //        [ 0.+2.j,  2.-1.j]])
            // >>> # with UPLO='L' this is numerically equivalent to using LA.eigvals()
            // >>> # with:
            // >>> b = np.array([[5.+0.j, 0.-2.j], [0.+2.j, 2.-0.j]])
            // >>> b
            // array([[ 5.+0.j,  0.-2.j],
            //        [ 0.+2.j,  2.+0.j]])
            // >>> wa = LA.eigvalsh(a)
            // >>> wb = LA.eigvals(b)
            // >>> wa; wb
            // array([ 1.,  6.])
            // array([ 6.+0.j,  1.+0.j])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  # demonstrate the treatment of the imaginary part of the diagonal;
            given=  a = np.array([[5+2j, 9-2j], [0+2j, 2-1j]]);
            given=  a;
            expected=
                "array([[ 5.+2.j,  9.-2.j],\n" +
                "       [ 0.+2.j,  2.-1.j]])";
            Assert.AreEqual(expected, given.repr);
            given=  # with UPLO='L' this is numerically equivalent to using LA.eigvals();
            given=  # with:;
            given=  b = np.array([[5.+0.j, 0.-2.j], [0.+2.j, 2.-0.j]]);
            given=  b;
            expected=
                "array([[ 5.+0.j,  0.-2.j],\n" +
                "       [ 0.+2.j,  2.+0.j]])";
            Assert.AreEqual(expected, given.repr);
            given=  wa = LA.eigvalsh(a);
            given=  wb = LA.eigvals(b);
            given=  wa; wb;
            expected=
                "array([ 1.,  6.])\n" +
                "array([ 6.+0.j,  1.+0.j])";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        [TestMethod]
        public void invTest()
        {
            // >>> from numpy.linalg import inv
            // >>> a = np.array([[1., 2.], [3., 4.]])
            // >>> ainv = inv(a)
            // >>> np.allclose(np.dot(a, ainv), np.eye(2))
            // True
            // >>> np.allclose(np.dot(ainv, a), np.eye(2))
            // True
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  from numpy.linalg import inv;
            given=  a = np.array([[1., 2.], [3., 4.]]);
            given=  ainv = inv(a);
            given=  np.allclose(np.dot(a, ainv), np.eye(2));
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            given=  np.allclose(np.dot(ainv, a), np.eye(2));
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            #endif
            // If a is a matrix object, then the return value is a matrix as well:
            
            // >>> ainv = inv(np.matrix(a))
            // >>> ainv
            // matrix([[-2. ,  1. ],
            //         [ 1.5, -0.5]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  ainv = inv(np.matrix(a));
            given=  ainv;
            expected=
                "matrix([[-2. ,  1. ],\n" +
                "        [ 1.5, -0.5]])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Inverses of several matrices can be computed at once:
            
            // >>> a = np.array([[[1., 2.], [3., 4.]], [[1, 3], [3, 5]]])
            // >>> inv(a)
            // array([[[-2. ,  1. ],
            //         [ 1.5, -0.5]],
            //        [[-5. ,  2. ],
            //         [ 3. , -1. ]]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  a = np.array([[[1., 2.], [3., 4.]], [[1, 3], [3, 5]]]);
            given=  inv(a);
            expected=
                "array([[[-2. ,  1. ],\n" +
                "        [ 1.5, -0.5]],\n" +
                "       [[-5. ,  2. ],\n" +
                "        [ 3. , -1. ]]])";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        [TestMethod]
        public void lstsqTest()
        {
            // Fit a line, y = mx + c, through some noisy data-points:
            
            // >>> x = np.array([0, 1, 2, 3])
            // >>> y = np.array([-1, 0.2, 0.9, 2.1])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  x = np.array([0, 1, 2, 3]);
            given=  y = np.array([-1, 0.2, 0.9, 2.1]);
            #endif
            // By examining the coefficients, we see that the line should have a
            // gradient of roughly 1 and cut the y-axis at, more or less, -1.
            
            // We can rewrite the line equation as y = Ap, where A = [[x 1]]
            // and p = [[m], [c]].  Now use lstsq to solve for p:
            
            // >>> A = np.vstack([x, np.ones(len(x))]).T
            // >>> A
            // array([[ 0.,  1.],
            //        [ 1.,  1.],
            //        [ 2.,  1.],
            //        [ 3.,  1.]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  A = np.vstack([x, np.ones(len(x))]).T;
            given=  A;
            expected=
                "array([[ 0.,  1.],\n" +
                "       [ 1.,  1.],\n" +
                "       [ 2.,  1.],\n" +
                "       [ 3.,  1.]])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // >>> m, c = np.linalg.lstsq(A, y, rcond=None)[0]
            // >>> print(m, c)
            // 1.0 -0.95
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  m, c = np.linalg.lstsq(A, y, rcond=None)[0];
            given=  print(m, c);
            expected=
                "1.0 -0.95";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Plot the data along with the fitted line:
            
            // >>> import matplotlib.pyplot as plt
            // >>> plt.plot(x, y, 'o', label='Original data', markersize=10)
            // >>> plt.plot(x, m*x + c, 'r', label='Fitted line')
            // >>> plt.legend()
            // >>> plt.show()
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  import matplotlib.pyplot as plt;
            given=  plt.plot(x, y, 'o', label='Original data', markersize=10);
            given=  plt.plot(x, m*x + c, 'r', label='Fitted line');
            given=  plt.legend();
            given=  plt.show();
            #endif
        }

        [TestMethod]
        public void normTest()
        {
            // >>> from numpy import linalg as LA
            // >>> a = np.arange(9) - 4
            // >>> a
            // array([-4, -3, -2, -1,  0,  1,  2,  3,  4])
            // >>> b = a.reshape((3, 3))
            // >>> b
            // array([[-4, -3, -2],
            //        [-1,  0,  1],
            //        [ 2,  3,  4]])
            // 
            var  a = np.arange(9) - 4;
            NDarray given=  a;
            var expected=
                "array([-4, -3, -2, -1,  0,  1,  2,  3,  4])";
            Assert.AreEqual(expected, given.repr);
            var  b = a.reshape(3, 3);
            given=  b;
            expected=
                "array([[-4, -3, -2],\n" +
                "       [-1,  0,  1],\n" +
                "       [ 2,  3,  4]])";
            Assert.AreEqual(expected, given.repr);

            // >>> LA.norm(a)
            // 7.745966692414834
            // >>> LA.norm(b)
            // 7.745966692414834
            // >>> LA.norm(b, 'fro')
            // 7.745966692414834
            // >>> LA.norm(a, np.inf)
            // 4.0
            // >>> LA.norm(b, np.inf)
            // 9.0
            // >>> LA.norm(a, -np.inf)
            // 0.0
            // >>> LA.norm(b, -np.inf)
            // 2.0
            // 
            
            Assert.GreaterOrEqual(7.74596669f, LA.norm(a));
            Assert.GreaterOrEqual(7.74596669f, LA.norm(b));
            Assert.GreaterOrEqual(7.74596669f, LA.norm(b, "fro"));
            Assert.AreEqual(4, LA.norm(a, Constants.inf));
            Assert.AreEqual(9, LA.norm(b, Constants.inf));
            Assert.AreEqual(0, LA.norm(a, Constants.neg_inf));
            Assert.AreEqual(2, LA.norm(b, Constants.neg_inf));

            // >>> LA.norm(a, 1)
            // 20.0
            // >>> LA.norm(b, 1)
            // 7.0
            // >>> LA.norm(a, -1)
            // -4.6566128774142013e-010
            // >>> LA.norm(b, -1)
            // 6.0
            // >>> LA.norm(a, 2)
            // 7.745966692414834
            // >>> LA.norm(b, 2)
            // 7.3484692283495345
            // 

            Assert.AreEqual(20, LA.norm(a, 1));
            Assert.AreEqual(7, LA.norm(b, 1));
            Assert.GreaterOrEqual(0f, LA.norm(a, -1));
            Assert.GreaterOrEqual(6, LA.norm(b, -1));
            Assert.GreaterOrEqual(7.74596669f, LA.norm(a, 2));
            Assert.GreaterOrEqual(7.34846922f, LA.norm(b, 2));

            // >>> LA.norm(a, -2)
            // nan
            // >>> LA.norm(b, -2)
            // 1.8570331885190563e-016
            // >>> LA.norm(a, 3)
            // 5.8480354764257312
            // >>> LA.norm(a, -3)
            // nan
            // 

#if TODO
            object given = null;
            object expected = null;
            given=  LA.norm(a, -2);
            expected=
                "nan";
            Assert.AreEqual(expected, given.repr);
            given=  LA.norm(b, -2);
            expected=
                "1.8570331885190563e-016";
            Assert.AreEqual(expected, given.repr);
            given=  LA.norm(a, 3);
            expected=
                "5.8480354764257312";
            Assert.AreEqual(expected, given.repr);
            given=  LA.norm(a, -3);
            expected=
                "nan";
            Assert.AreEqual(expected, given.repr);
#endif
            // Using the axis argument to compute vector norms:

            // >>> c = np.array([[ 1, 2, 3],
            // ...               [-1, 1, 4]])
            // >>> LA.norm(c, axis=0)
            // array([ 1.41421356,  2.23606798,  5.        ])
            // >>> LA.norm(c, axis=1)
            // array([ 3.74165739,  4.24264069])
            // >>> LA.norm(c, ord=1, axis=1)
            // array([ 6.,  6.])
            // 

#if TODO
            object given = null;
            object expected = null;
            given=  c = np.array([[ 1, 2, 3],;
            expected=
                "...               [-1, 1, 4]])";
            Assert.AreEqual(expected, given.repr);
            given=  LA.norm(c, axis=0);
            expected=
                "array([ 1.41421356,  2.23606798,  5.        ])";
            Assert.AreEqual(expected, given.repr);
            given=  LA.norm(c, axis=1);
            expected=
                "array([ 3.74165739,  4.24264069])";
            Assert.AreEqual(expected, given.repr);
            given=  LA.norm(c, ord=1, axis=1);
            expected=
                "array([ 6.,  6.])";
            Assert.AreEqual(expected, given.repr);
#endif
            // Using the axis argument to compute matrix norms:

            // >>> m = np.arange(8).reshape(2,2,2)
            // >>> LA.norm(m, axis=(1,2))
            // array([  3.74165739,  11.22497216])
            // >>> LA.norm(m[0, :, :]), LA.norm(m[1, :, :])
            // (3.7416573867739413, 11.224972160321824)
            // 

#if TODO
            object given = null;
            object expected = null;
            given=  m = np.arange(8).reshape(2,2,2);
            given=  LA.norm(m, axis=(1,2));
            expected=
                "array([  3.74165739,  11.22497216])";
            Assert.AreEqual(expected, given.repr);
            given=  LA.norm(m[0, :, :]), LA.norm(m[1, :, :]);
            expected=
                "(3.7416573867739413, 11.224972160321824)";
            Assert.AreEqual(expected, given.repr);
#endif
        }
        [TestMethod]
        public void pinvTest()
        {
            // The following example checks that a * a+ * a == a and
            // a+ * a * a+ == a+:
            
            // >>> a = np.random.randn(9, 6)
            // >>> B = np.linalg.pinv(a)
            // >>> np.allclose(a, np.dot(a, np.dot(B, a)))
            // True
            // >>> np.allclose(B, np.dot(B, np.dot(a, B)))
            // True
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  a = np.random.randn(9, 6);
            given=  B = np.linalg.pinv(a);
            given=  np.allclose(a, np.dot(a, np.dot(B, a)));
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            given=  np.allclose(B, np.dot(B, np.dot(a, B)));
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        [TestMethod]
        public void solveTest()
        {
            // Solve the system of equations 3 * x0 + x1 = 9 and x0 + 2 * x1 = 8:
            
            // >>> a = np.array([[3,1], [1,2]])
            // >>> b = np.array([9,8])
            // >>> x = np.linalg.solve(a, b)
            // >>> x
            // array([ 2.,  3.])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  a = np.array([[3,1], [1,2]]);
            given=  b = np.array([9,8]);
            given=  x = np.linalg.solve(a, b);
            given=  x;
            expected=
                "array([ 2.,  3.])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Check that the solution is correct:
            
            // >>> np.allclose(np.dot(a, x), b)
            // True
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  np.allclose(np.dot(a, x), b);
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        [TestMethod]
        public void svdTest()
        {
            // >>> a = np.random.randn(9, 6) + 1j*np.random.randn(9, 6)
            // >>> b = np.random.randn(2, 7, 8, 3) + 1j*np.random.randn(2, 7, 8, 3)
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  a = np.random.randn(9, 6) + 1j*np.random.randn(9, 6);
            given=  b = np.random.randn(2, 7, 8, 3) + 1j*np.random.randn(2, 7, 8, 3);
            #endif
            // Reconstruction based on full SVD, 2D case:
            
            // >>> u, s, vh = np.linalg.svd(a, full_matrices=True)
            // >>> u.shape, s.shape, vh.shape
            // ((9, 9), (6,), (6, 6))
            // >>> np.allclose(a, np.dot(u[:, :6] * s, vh))
            // True
            // >>> smat = np.zeros((9, 6), dtype=complex)
            // >>> smat[:6, :6] = np.diag(s)
            // >>> np.allclose(a, np.dot(u, np.dot(smat, vh)))
            // True
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  u, s, vh = np.linalg.svd(a, full_matrices=True);
            given=  u.shape, s.shape, vh.shape;
            expected=
                "((9, 9), (6,), (6, 6))";
            Assert.AreEqual(expected, given.repr);
            given=  np.allclose(a, np.dot(u[:, :6] * s, vh));
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            given=  smat = np.zeros((9, 6), dtype=complex);
            given=  smat[:6, :6] = np.diag(s);
            given=  np.allclose(a, np.dot(u, np.dot(smat, vh)));
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Reconstruction based on reduced SVD, 2D case:
            
            // >>> u, s, vh = np.linalg.svd(a, full_matrices=False)
            // >>> u.shape, s.shape, vh.shape
            // ((9, 6), (6,), (6, 6))
            // >>> np.allclose(a, np.dot(u * s, vh))
            // True
            // >>> smat = np.diag(s)
            // >>> np.allclose(a, np.dot(u, np.dot(smat, vh)))
            // True
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  u, s, vh = np.linalg.svd(a, full_matrices=False);
            given=  u.shape, s.shape, vh.shape;
            expected=
                "((9, 6), (6,), (6, 6))";
            Assert.AreEqual(expected, given.repr);
            given=  np.allclose(a, np.dot(u * s, vh));
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            given=  smat = np.diag(s);
            given=  np.allclose(a, np.dot(u, np.dot(smat, vh)));
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Reconstruction based on full SVD, 4D case:
            
            // >>> u, s, vh = np.linalg.svd(b, full_matrices=True)
            // >>> u.shape, s.shape, vh.shape
            // ((2, 7, 8, 8), (2, 7, 3), (2, 7, 3, 3))
            // >>> np.allclose(b, np.matmul(u[..., :3] * s[..., None, :], vh))
            // True
            // >>> np.allclose(b, np.matmul(u[..., :3], s[..., None] * vh))
            // True
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  u, s, vh = np.linalg.svd(b, full_matrices=True);
            given=  u.shape, s.shape, vh.shape;
            expected=
                "((2, 7, 8, 8), (2, 7, 3), (2, 7, 3, 3))";
            Assert.AreEqual(expected, given.repr);
            given=  np.allclose(b, np.matmul(u[..., :3] * s[..., None, :], vh));
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            given=  np.allclose(b, np.matmul(u[..., :3], s[..., None] * vh));
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Reconstruction based on reduced SVD, 4D case:
            
            // >>> u, s, vh = np.linalg.svd(b, full_matrices=False)
            // >>> u.shape, s.shape, vh.shape
            // ((2, 7, 8, 3), (2, 7, 3), (2, 7, 3, 3))
            // >>> np.allclose(b, np.matmul(u * s[..., None, :], vh))
            // True
            // >>> np.allclose(b, np.matmul(u, s[..., None] * vh))
            // True
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  u, s, vh = np.linalg.svd(b, full_matrices=False);
            given=  u.shape, s.shape, vh.shape;
            expected=
                "((2, 7, 8, 3), (2, 7, 3), (2, 7, 3, 3))";
            Assert.AreEqual(expected, given.repr);
            given=  np.allclose(b, np.matmul(u * s[..., None, :], vh));
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            given=  np.allclose(b, np.matmul(u, s[..., None] * vh));
            expected=
                "True";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        [TestMethod]
        public void fftTest()
        {
            // >>> np.fft.fft(np.exp(2j * np.pi * np.arange(8) / 8))
            // array([ -3.44505240e-16 +1.14383329e-17j,
            //          8.00000000e+00 -5.71092652e-15j,
            //          2.33482938e-16 +1.22460635e-16j,
            //          1.64863782e-15 +1.77635684e-15j,
            //          9.95839695e-17 +2.33482938e-16j,
            //          0.00000000e+00 +1.66837030e-15j,
            //          1.14383329e-17 +1.22460635e-16j,
            //          -1.64863782e-15 +1.77635684e-15j])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  np.fft.fft(np.exp(2j * np.pi * np.arange(8) / 8));
            expected=
                "array([ -3.44505240e-16 +1.14383329e-17j,\n" +
                "         8.00000000e+00 -5.71092652e-15j,\n" +
                "         2.33482938e-16 +1.22460635e-16j,\n" +
                "         1.64863782e-15 +1.77635684e-15j,\n" +
                "         9.95839695e-17 +2.33482938e-16j,\n" +
                "         0.00000000e+00 +1.66837030e-15j,\n" +
                "         1.14383329e-17 +1.22460635e-16j,\n" +
                "         -1.64863782e-15 +1.77635684e-15j])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // In this example, real input has an FFT which is Hermitian, i.e., symmetric
            // in the real part and anti-symmetric in the imaginary part, as described in
            // the numpy.fft documentation:
            
            // >>> import matplotlib.pyplot as plt
            // >>> t = np.arange(256)
            // >>> sp = np.fft.fft(np.sin(t))
            // >>> freq = np.fft.fftfreq(t.shape[-1])
            // >>> plt.plot(freq, sp.real, freq, sp.imag)
            // [<matplotlib.lines.Line2D object at 0x...>, <matplotlib.lines.Line2D object at 0x...>]
            // >>> plt.show()
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  import matplotlib.pyplot as plt;
            given=  t = np.arange(256);
            given=  sp = np.fft.fft(np.sin(t));
            given=  freq = np.fft.fftfreq(t.shape[-1]);
            given=  plt.plot(freq, sp.real, freq, sp.imag);
            expected=
                "[<matplotlib.lines.Line2D object at 0x...>, <matplotlib.lines.Line2D object at 0x...>]";
            Assert.AreEqual(expected, given.repr);
            given=  plt.show();
            #endif
        }
        [TestMethod]
        public void fft2Test()
        {
            // >>> a = np.mgrid[:5, :5][0]
            // >>> np.fft.fft2(a)
            // array([[ 50.0 +0.j        ,   0.0 +0.j        ,   0.0 +0.j        ,
            //           0.0 +0.j        ,   0.0 +0.j        ],
            //        [-12.5+17.20477401j,   0.0 +0.j        ,   0.0 +0.j        ,
            //           0.0 +0.j        ,   0.0 +0.j        ],
            //        [-12.5 +4.0614962j ,   0.0 +0.j        ,   0.0 +0.j        ,
            //           0.0 +0.j        ,   0.0 +0.j        ],
            //        [-12.5 -4.0614962j ,   0.0 +0.j        ,   0.0 +0.j        ,
            //             0.0 +0.j        ,   0.0 +0.j        ],
            //        [-12.5-17.20477401j,   0.0 +0.j        ,   0.0 +0.j        ,
            //           0.0 +0.j        ,   0.0 +0.j        ]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  a = np.mgrid[:5, :5][0];
            given=  np.fft.fft2(a);
            expected=
                "array([[ 50.0 +0.j        ,   0.0 +0.j        ,   0.0 +0.j        ,\n" +
                "          0.0 +0.j        ,   0.0 +0.j        ],\n" +
                "       [-12.5+17.20477401j,   0.0 +0.j        ,   0.0 +0.j        ,\n" +
                "          0.0 +0.j        ,   0.0 +0.j        ],\n" +
                "       [-12.5 +4.0614962j ,   0.0 +0.j        ,   0.0 +0.j        ,\n" +
                "          0.0 +0.j        ,   0.0 +0.j        ],\n" +
                "       [-12.5 -4.0614962j ,   0.0 +0.j        ,   0.0 +0.j        ,\n" +
                "            0.0 +0.j        ,   0.0 +0.j        ],\n" +
                "       [-12.5-17.20477401j,   0.0 +0.j        ,   0.0 +0.j        ,\n" +
                "          0.0 +0.j        ,   0.0 +0.j        ]])";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        [TestMethod]
        public void fftnTest()
        {
            // >>> a = np.mgrid[:3, :3, :3][0]
            // >>> np.fft.fftn(a, axes=(1, 2))
            // array([[[  0.+0.j,   0.+0.j,   0.+0.j],
            //         [  0.+0.j,   0.+0.j,   0.+0.j],
            //         [  0.+0.j,   0.+0.j,   0.+0.j]],
            //        [[  9.+0.j,   0.+0.j,   0.+0.j],
            //         [  0.+0.j,   0.+0.j,   0.+0.j],
            //         [  0.+0.j,   0.+0.j,   0.+0.j]],
            //        [[ 18.+0.j,   0.+0.j,   0.+0.j],
            //         [  0.+0.j,   0.+0.j,   0.+0.j],
            //         [  0.+0.j,   0.+0.j,   0.+0.j]]])
            // >>> np.fft.fftn(a, (2, 2), axes=(0, 1))
            // array([[[ 2.+0.j,  2.+0.j,  2.+0.j],
            //         [ 0.+0.j,  0.+0.j,  0.+0.j]],
            //        [[-2.+0.j, -2.+0.j, -2.+0.j],
            //         [ 0.+0.j,  0.+0.j,  0.+0.j]]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  a = np.mgrid[:3, :3, :3][0];
            given=  np.fft.fftn(a, axes=(1, 2));
            expected=
                "array([[[  0.+0.j,   0.+0.j,   0.+0.j],\n" +
                "        [  0.+0.j,   0.+0.j,   0.+0.j],\n" +
                "        [  0.+0.j,   0.+0.j,   0.+0.j]],\n" +
                "       [[  9.+0.j,   0.+0.j,   0.+0.j],\n" +
                "        [  0.+0.j,   0.+0.j,   0.+0.j],\n" +
                "        [  0.+0.j,   0.+0.j,   0.+0.j]],\n" +
                "       [[ 18.+0.j,   0.+0.j,   0.+0.j],\n" +
                "        [  0.+0.j,   0.+0.j,   0.+0.j],\n" +
                "        [  0.+0.j,   0.+0.j,   0.+0.j]]])";
            Assert.AreEqual(expected, given.repr);
            given=  np.fft.fftn(a, (2, 2), axes=(0, 1));
            expected=
                "array([[[ 2.+0.j,  2.+0.j,  2.+0.j],\n" +
                "        [ 0.+0.j,  0.+0.j,  0.+0.j]],\n" +
                "       [[-2.+0.j, -2.+0.j, -2.+0.j],\n" +
                "        [ 0.+0.j,  0.+0.j,  0.+0.j]]])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // >>> import matplotlib.pyplot as plt
            // >>> [X, Y] = np.meshgrid(2 * np.pi * np.arange(200) / 12,
            // ...                      2 * np.pi * np.arange(200) / 34)
            // >>> S = np.sin(X) + np.cos(Y) + np.random.uniform(0, 1, X.shape)
            // >>> FS = np.fft.fftn(S)
            // >>> plt.imshow(np.log(np.abs(np.fft.fftshift(FS))**2))
            // <matplotlib.image.AxesImage object at 0x...>
            // >>> plt.show()
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  import matplotlib.pyplot as plt;
            given=  [X, Y] = np.meshgrid(2 * np.pi * np.arange(200) / 12,;
            expected=
                "...                      2 * np.pi * np.arange(200) / 34)";
            Assert.AreEqual(expected, given.repr);
            given=  S = np.sin(X) + np.cos(Y) + np.random.uniform(0, 1, X.shape);
            given=  FS = np.fft.fftn(S);
            given=  plt.imshow(np.log(np.abs(np.fft.fftshift(FS))**2));
            expected=
                "<matplotlib.image.AxesImage object at 0x...>";
            Assert.AreEqual(expected, given.repr);
            given=  plt.show();
            #endif
        }
        [TestMethod]
        public void ifftTest()
        {
            // >>> np.fft.ifft([0, 4, 0, 0])
            // array([ 1.+0.j,  0.+1.j, -1.+0.j,  0.-1.j])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  np.fft.ifft([0, 4, 0, 0]);
            expected=
                "array([ 1.+0.j,  0.+1.j, -1.+0.j,  0.-1.j])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Create and plot a band-limited signal with random phases:
            
            // >>> import matplotlib.pyplot as plt
            // >>> t = np.arange(400)
            // >>> n = np.zeros((400,), dtype=complex)
            // >>> n[40:60] = np.exp(1j*np.random.uniform(0, 2*np.pi, (20,)))
            // >>> s = np.fft.ifft(n)
            // >>> plt.plot(t, s.real, 'b-', t, s.imag, 'r--')
            // ...
            // >>> plt.legend(('real', 'imaginary'))
            // ...
            // >>> plt.show()
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  import matplotlib.pyplot as plt;
            given=  t = np.arange(400);
            given=  n = np.zeros((400,), dtype=complex);
            given=  n[40:60] = np.exp(1j*np.random.uniform(0, 2*np.pi, (20,)));
            given=  s = np.fft.ifft(n);
            given=  plt.plot(t, s.real, 'b-', t, s.imag, 'r--');
            expected=
                "...";
            Assert.AreEqual(expected, given.repr);
            given=  plt.legend(('real', 'imaginary'));
            expected=
                "...";
            Assert.AreEqual(expected, given.repr);
            given=  plt.show();
            #endif
        }
        [TestMethod]
        public void ifft2Test()
        {
            // >>> a = 4 * np.eye(4)
            // >>> np.fft.ifft2(a)
            // array([[ 1.+0.j,  0.+0.j,  0.+0.j,  0.+0.j],
            //        [ 0.+0.j,  0.+0.j,  0.+0.j,  1.+0.j],
            //        [ 0.+0.j,  0.+0.j,  1.+0.j,  0.+0.j],
            //        [ 0.+0.j,  1.+0.j,  0.+0.j,  0.+0.j]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  a = 4 * np.eye(4);
            given=  np.fft.ifft2(a);
            expected=
                "array([[ 1.+0.j,  0.+0.j,  0.+0.j,  0.+0.j],\n" +
                "       [ 0.+0.j,  0.+0.j,  0.+0.j,  1.+0.j],\n" +
                "       [ 0.+0.j,  0.+0.j,  1.+0.j,  0.+0.j],\n" +
                "       [ 0.+0.j,  1.+0.j,  0.+0.j,  0.+0.j]])";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
        [TestMethod]
        public void ifftnTest()
        {
            // >>> a = np.eye(4)
            // >>> np.fft.ifftn(np.fft.fftn(a, axes=(0,)), axes=(1,))
            // array([[ 1.+0.j,  0.+0.j,  0.+0.j,  0.+0.j],
            //        [ 0.+0.j,  1.+0.j,  0.+0.j,  0.+0.j],
            //        [ 0.+0.j,  0.+0.j,  1.+0.j,  0.+0.j],
            //        [ 0.+0.j,  0.+0.j,  0.+0.j,  1.+0.j]])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  a = np.eye(4);
            given=  np.fft.ifftn(np.fft.fftn(a, axes=(0,)), axes=(1,));
            expected=
                "array([[ 1.+0.j,  0.+0.j,  0.+0.j,  0.+0.j],\n" +
                "       [ 0.+0.j,  1.+0.j,  0.+0.j,  0.+0.j],\n" +
                "       [ 0.+0.j,  0.+0.j,  1.+0.j,  0.+0.j],\n" +
                "       [ 0.+0.j,  0.+0.j,  0.+0.j,  1.+0.j]])";
            Assert.AreEqual(expected, given.repr);
            #endif
            // Create and plot an image with band-limited frequency content:
            
            // >>> import matplotlib.pyplot as plt
            // >>> n = np.zeros((200,200), dtype=complex)
            // >>> n[60:80, 20:40] = np.exp(1j*np.random.uniform(0, 2*np.pi, (20, 20)))
            // >>> im = np.fft.ifftn(n).real
            // >>> plt.imshow(im)
            // <matplotlib.image.AxesImage object at 0x...>
            // >>> plt.show()
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  import matplotlib.pyplot as plt;
            given=  n = np.zeros((200,200), dtype=complex);
            given=  n[60:80, 20:40] = np.exp(1j*np.random.uniform(0, 2*np.pi, (20, 20)));
            given=  im = np.fft.ifftn(n).real;
            given=  plt.imshow(im);
            expected=
                "<matplotlib.image.AxesImage object at 0x...>";
            Assert.AreEqual(expected, given.repr);
            given=  plt.show();
            #endif
        }
        [TestMethod]
        public void i0Test()
        {
            // >>> np.i0([0.])
            // array(1.0)
            // >>> np.i0([0., 1. + 2j])
            // array([ 1.00000000+0.j        ,  0.18785373+0.64616944j])
            // 
            
            #if TODO
            object given = null;
            object expected = null;
            given=  np.i0([0.]);
            expected=
                "array(1.0)";
            Assert.AreEqual(expected, given.repr);
            given=  np.i0([0., 1. + 2j]);
            expected=
                "array([ 1.00000000+0.j        ,  0.18785373+0.64616944j])";
            Assert.AreEqual(expected, given.repr);
            #endif
        }
    }
}
