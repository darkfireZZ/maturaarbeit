using System;
using System.Linq;

namespace Cubing
{
    //IMPR tests
    //IMPR names
    /// <summary>
    /// An inefficient implementation of linear regression.
    /// </summary>
    public static class LinearRegression
    {
        //IMPR change documentation for initialTheta
        //IMPR documentation
        /// <summary>
        /// Learn on a training set with gradient descent for a specific
        /// number of iterations.
        /// </summary>
        /// <remarks>
        /// Use <seealso cref="Predict(double[,], double[])"/> to predict
        /// new examples with the calculated value for theta.
        /// See <seealso cref="Cost(double[,], double[], double[])"/> for
        /// calculating the cost. And see
        /// <seealso cref="Gradient(double[,], double[], double[])"/> for
        /// calculating the gradient.
        /// </remarks>
        /// <param name="features">
        /// The training examples. <c>features[m, n]</c> refers to the
        /// n-feature of the m-th training example.
        /// </param>
        /// <param name="labels">
        /// <c>labels[m]</c> is the value that should be predicted for the
        /// m-th training example.
        /// </param>
        /// <param name="initialTheta">
        /// The initial value for theta. If
        /// <paramref name="initialTheta"/> set to null theta is
        /// initialized to an array of zeros.
        /// </param>
        /// <param name="learningRate">
        /// The learning rate for gradient Descent.
        /// </param>
        /// <param name="numberOfIterations">
        /// The number of times gradient descent is performed.
        /// </param>
        /// <returns>
        /// The calculated theta.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <c>features.GetLength(0) != labels.Length</c> or if
        /// <c>features.GetLength(1) != initialTheta.Length</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="features"/> or
        /// <paramref name="labels"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="learningRate"/> or
        /// <paramref name="numberOfIterations"/> is smaller than zero.
        /// </exception>
        public static double[] Learn(double[,] features, double[] labels, double[] initialTheta = null, double learningRate = 1.0d, int numberOfIterations = 500)
        {
            if (features is null)
                throw new ArgumentNullException(nameof(features) + " is null.");
            if (labels is null)
                throw new ArgumentNullException(nameof(labels) + " is null.");
            if (numberOfIterations < 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfIterations) + " is smaller than 0: " + numberOfIterations);
            if (learningRate <= 0.0d)
                throw new ArgumentOutOfRangeException(nameof(learningRate) + " is smaller than 0.0: " + learningRate);
            if (initialTheta is null)
            {
                int numFeatures = features.GetLength(1);
                initialTheta = new double[numFeatures];
                for (int index = 0; index < numFeatures; index++)
                    initialTheta[index] = 0.0d;
            }
            else if (features.GetLength(1) != initialTheta.Length)
                throw new ArgumentException(nameof(features) + " and " + nameof(initialTheta) + " have a different number of features.");
            if (features.GetLength(0) != labels.Length)
                throw new ArgumentException(nameof(features) + " and " + nameof(labels) + " have a different number of examples.");

            double[] theta = (double[])initialTheta.Clone();
            for (int iteration = 0; iteration < numberOfIterations; iteration++)
            {
                double[] gradient = Gradient(features, labels, theta);
                theta = theta
                    .Select((element, index) => element - learningRate * gradient[index])
                    .ToArray();
            }
            return theta;
        }

        /// <summary>
        /// Calculate the squared error on a training set for a specific
        /// value of theta. This function is minimized by
        /// <see cref="Learn(double[,], double[], double[], double, int)"/>.
        /// </summary>
        /// <param name="features">
        /// The training examples. <c>features[m, n]</c> refers to the
        /// n-feature of the m-th training example.
        /// </param>
        /// <param name="labels">
        /// <c>labels[m]</c> is the value that should be predicted for the
        /// m-th training example.
        /// </param>
        /// <param name="theta">
        /// The theta to calculate the error for.
        /// </param>
        /// <returns>
        /// The squared error on the training set for the specified value
        /// of <paramref name="theta"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <c>features.GetLength(0) != labels.Length</c> or if
        /// <c>features.GetLength(1) != theta.Length</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="features"/>,
        /// <paramref name="labels"/> or <paramref name="theta"/> is null.
        /// </exception>
        public static double Cost(double[,] features, double[] labels, double[] theta)
        {
            if (features is null)
                throw new ArgumentNullException(nameof(features) + " is null.");
            if (labels is null)
                throw new ArgumentNullException(nameof(labels) + " is null.");
            if (theta is null)
                throw new ArgumentNullException(nameof(theta) + " is null.");
            if (features.GetLength(0) != labels.Length)
                throw new ArgumentException(nameof(features) + " and " + nameof(labels) + " have a different number of examples.");
            if (features.GetLength(1) != theta.Length)
                throw new ArgumentException(nameof(features) + " and " + nameof(theta) + " have a different number of features.");

            return (1d / (2 * features.GetLength(0))) * Predict(features, theta)
                .Select((value, index) => (value - labels[index]) * (value - labels[index]))
                .Sum();
        }

        /// <summary>
        /// Calculate the absolute error on a training set for a specific
        /// value of theta. This is not the function minimized by
        /// <see cref="Learn(double[,], double[], double[], double, int)"/>.
        /// </summary>
        /// <param name="features">
        /// The training examples. <c>features[m, n]</c> refers to the
        /// n-feature of the m-th training example.
        /// </param>
        /// <param name="labels">
        /// <c>labels[m]</c> is the value that should be predicted for the
        /// m-th training example.
        /// </param>
        /// <param name="theta">
        /// The theta to calculate the error for.
        /// </param>
        /// <returns>
        /// The absolute error on the training set for the specified value
        /// of <paramref name="theta"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <c>features.GetLength(0) != labels.Length</c> or if
        /// <c>features.GetLength(1) != theta.Length</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="features"/>,
        /// <paramref name="labels"/> or <paramref name="theta"/> is null.
        /// </exception>
        public static double AbsoluteCost(double[,] features, double[] labels, double[] theta)
        {
            if (features is null)
                throw new ArgumentNullException(nameof(features) + " is null.");
            if (labels is null)
                throw new ArgumentNullException(nameof(labels) + " is null.");
            if (theta is null)
                throw new ArgumentNullException(nameof(theta) + " is null.");
            if (features.GetLength(0) != labels.Length)
                throw new ArgumentException(nameof(features) + " and " + nameof(labels) + " have a different number of examples.");
            if (features.GetLength(1) != theta.Length)
                throw new ArgumentException(nameof(features) + " and " + nameof(theta) + " have a different number of features.");

            return (1d / features.GetLength(0)) * Predict(features, theta)
                .Select((value, index) => Math.Abs((value - labels[index])))
                .Sum();
        }

        /// <summary>
        /// Calculate the gradient on a training set for a specific value
        /// of theta.
        /// </summary>
        /// <param name="features">
        /// The training examples. <c>features[m, n]</c> refers to the
        /// n-feature of the m-th training example.
        /// </param>
        /// <param name="labels">
        /// <c>labels[m]</c> is the value that should be predicted for the
        /// m-th training example.
        /// </param>
        /// <param name="theta">
        /// The theta to calculate the gradient for.
        /// </param>
        /// <returns>
        /// The gradient on the training set for the specified value of
        /// <paramref name="theta"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <c>features.GetLength(0) != labels.Length</c> or if
        /// <c>features.GetLength(1) != theta.Length</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="features"/>,
        /// <paramref name="labels"/> or <paramref name="theta"/> is null.
        /// </exception>
        public static double[] Gradient(double[,] features, double[] labels, double[] theta)
        {
            if (features is null)
                throw new ArgumentNullException(nameof(features) + " is null.");
            if (labels is null)
                throw new ArgumentNullException(nameof(labels) + " is null.");
            if (theta is null)
                throw new ArgumentNullException(nameof(theta) + " is null.");
            if (features.GetLength(0) != labels.Length)
                throw new ArgumentException(nameof(features) + " and " + nameof(labels) + " have a different number of examples.");
            if (features.GetLength(1) != theta.Length)
                throw new ArgumentException(nameof(features) + " and " + nameof(theta) + " have a different number of features.");

            return theta
                .Select((featureElement, featureIndex) => Predict(features, theta)
                    .Select((predictionElement, predictionIndex) => (predictionElement - labels[predictionIndex]) * features[predictionIndex, featureIndex])
                    .Sum() / features.GetLength(0))
                .ToArray();
        }

        /// <summary>
        /// Predict values using a specific value of theta.
        /// </summary>
        /// <remarks>
        /// See <seealso cref="Learn(double[,], double[], double[], double, int)"/>
        /// for finding the best value for theta on a training set.
        /// </remarks>
        /// <param name="features">
        /// The examples to predict values for. <c>features[m, n]</c>
        /// refers to the n-feature of the m-th training example.
        /// </param>
        /// <param name="theta">
        /// The theta value used to predict the values.
        /// </param>
        /// <returns>
        /// The predicted values.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <c>features.GetLength(1) != theta.Length</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="features"/> or
        /// <paramref name="theta"/> is null.
        /// </exception>
        public static double[] Predict(double[,] features, double[] theta)
        {
            if (features is null)
                throw new ArgumentNullException(nameof(features) + " is null.");
            if (theta is null)
                throw new ArgumentNullException(nameof(theta) + " is null.");
            if (features.GetLength(1) != theta.Length)
                throw new ArgumentException(nameof(features) + " and " + nameof(theta) + " have a different number of features.");

            return new double[features.GetLength(0)]
                .Select((zero, trainingExampleIndex) => theta
                    .Select((element, index) => element * features[trainingExampleIndex, index])
                    .Sum())
                .ToArray();
        }
    }
}