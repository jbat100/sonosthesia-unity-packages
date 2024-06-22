Python code solving cubic polynomial for smooth position and velocity trajectory estimation:

```
import sympy as sp

# Define symbols
t1, t2, p1, p2, v1, v2 = sp.symbols('t1 t2 p1 p2 v1 v2')
a0, a1, a2, a3 = sp.symbols('a0 a1 a2 a3')

# Define equations
eq1 = sp.Eq(p1, a0 + a1*t1 + a2*t1**2 + a3*t1**3)
eq2 = sp.Eq(v1, a1 + 2*a2*t1 + 3*a3*t1**2)
eq3 = sp.Eq(p2, a0 + a1*t2 + a2*t2**2 + a3*t2**3)
eq4 = sp.Eq(v2, a1 + 2*a2*t2 + 3*a3*t2**2)

# Solve equations
solution = sp.solve((eq1, eq2, eq3, eq4), (a0, a1, a2, a3))

# Generate LaTeX code
latex_a0 = sp.latex(solution[a0])
latex_a1 = sp.latex(solution[a1])
latex_a2 = sp.latex(solution[a2])
latex_a3 = sp.latex(solution[a3])

print(f"a_0: {latex_a0}")
print(f"a_1: {latex_a1}")
print(f"a_2: {latex_a2}")
print(f"a_3: {latex_a3}")

ccode_a0 = sp.ccode(solution[a0])
ccode_a1 = sp.ccode(solution[a1])
ccode_a2 = sp.ccode(solution[a2])
ccode_a3 = sp.ccode(solution[a3])

print(f"a_0: {ccode_a0}")
print(f"a_1: {ccode_a1}")
print(f"a_2: {ccode_a2}")
print(f"a_3: {ccode_a3}")
```

Gives latex output:

a_0: 

$$
\frac{3 p_{1} t_{1} t_{2}^{2} - p_{1} t_{2}^{3} + p_{2} t_{1}^{3} - 3 p_{2} t_{1}^{2} t_{2} - t_{1}^{3} t_{2} v_{2} - t_{1}^{2} t_{2}^{2} v_{1} + t_{1}^{2} t_{2}^{2} v_{2} + t_{1} t_{2}^{3} v_{1}}{t_{1}^{3} - 3 t_{1}^{2} t_{2} + 3 t_{1} t_{2}^{2} - t_{2}^{3}}
$$

$$
a_1: \frac{- 6 p_{1} t_{1} t_{2} + 6 p_{2} t_{1} t_{2} + t_{1}^{3} v_{2} + 2 t_{1}^{2} t_{2} v_{1} + t_{1}^{2} t_{2} v_{2} - t_{1} t_{2}^{2} v_{1} - 2 t_{1} t_{2}^{2} v_{2} - t_{2}^{3} v_{1}}{t_{1}^{3} - 3 t_{1}^{2} t_{2} + 3 t_{1} t_{2}^{2} - t_{2}^{3}}
$$

$$
a_2: \frac{3 p_{1} t_{1} + 3 p_{1} t_{2} - 3 p_{2} t_{1} - 3 p_{2} t_{2} - t_{1}^{2} v_{1} - 2 t_{1}^{2} v_{2} - t_{1} t_{2} v_{1} + t_{1} t_{2} v_{2} + 2 t_{2}^{2} v_{1} + t_{2}^{2} v_{2}}{t_{1}^{3} - 3 t_{1}^{2} t_{2} + 3 t_{1} t_{2}^{2} - t_{2}^{3}}
$$

$$
a_3: \frac{- 2 p_{1} + 2 p_{2} + t_{1} v_{1} + t_{1} v_{2} - t_{2} v_{1} - t_{2} v_{2}}{t_{1}^{3} - 3 t_{1}^{2} t_{2} + 3 t_{1} t_{2}^{2} - t_{2}^{3}}
$$

And C code:

a_0:

```
(3*p1*t1*pow(t2, 2) - p1*pow(t2, 3) + p2*pow(t1, 3) - 3*p2*pow(t1, 2)*t2 - pow(t1, 3)*t2*v2 - pow(t1, 2)*pow(t2, 2)*v1 + pow(t1, 2)*pow(t2, 2)*v2 + t1*pow(t2, 3)*v1)/(pow(t1, 3) - 3*pow(t1, 2)*t2 + 3*t1*pow(t2, 2) - pow(t2, 3))
```

a_1:

```
(-6*p1*t1*t2 + 6*p2*t1*t2 + pow(t1, 3)*v2 + 2*pow(t1, 2)*t2*v1 + pow(t1, 2)*t2*v2 - t1*pow(t2, 2)*v1 - 2*t1*pow(t2, 2)*v2 - pow(t2, 3)*v1)/(pow(t1, 3) - 3*pow(t1, 2)*t2 + 3*t1*pow(t2, 2) - pow(t2, 3))
```

a_2:

```
(3*p1*t1 + 3*p1*t2 - 3*p2*t1 - 3*p2*t2 - pow(t1, 2)*v1 - 2*pow(t1, 2)*v2 - t1*t2*v1 + t1*t2*v2 + 2*pow(t2, 2)*v1 + pow(t2, 2)*v2)/(pow(t1, 3) - 3*pow(t1, 2)*t2 + 3*t1*pow(t2, 2) - pow(t2, 3))
```

a_3:

```
(-2*p1 + 2*p2 + t1*v1 + t1*v2 - t2*v1 - t2*v2)/(pow(t1, 3) - 3*pow(t1, 2)*t2 + 3*t1*pow(t2, 2) - pow(t2, 3))
```

Original ChatGPT conversation result which is false but pointed me in the right direction:

Using a cubic polynomial trajectory planning method, which ensures smooth changes in both position and velocity.

Here are the final equations:

$$
a_3 = \frac{2(t_2 - t_1)(p_2 - p_1) - (v_2 t_2 - v_1 t_1)(t_2 - t_1) - \frac{(v_1 - v_2)(t_2^2 - t_1^2)}{2}}{3(t_2^2 - t_1^2)^2 + 2(t_2^3 - t_1^3)(t_2 - t_1)}
$$

$$
a_2 = \frac{v_1 - v_2 - 3a_3 (t_2^2 - t_1^2)}{2(t_2 - t_1)}
$$

$$
a_1 = v_1 - 2a_2 t_1 - 3a_3 t_1^2
$$

$$
a_0 = p_1 - a_1 t_1 - a_2 t_1^2 - a_3 t_1^3
$$

Here is the derivation:

First, we have the cubic polynomial:

$$
p(t) = a_0 + a_1 t + a_2 t^2 + a_3 t^3
$$

And its derivative for velocity:

$$
v(t) = \frac{dp(t)}{dt} = a_1 + 2a_2 t + 3a_3 t^2
$$

Using the boundary conditions:

$$
p(t_1) = p_1
$$

$$
v(t_1) = v_1
$$

$$
p(t_2) = p_2
$$

$$
v(t_2) = v_2
$$

We set up the following equations:

1. $p_1 = a_0 + a_1 t_1 + a_2 t_1^2 + a_3 t_1^3$
2. $v_1 = a_1 + 2a_2 t_1 + 3a_3 t_1^2$
3. $p_2 = a_0 + a_1 t_2 + a_2 t_2^2 + a_3 t_2^3$
4. $v_2 = a_1 + 2a_2 t_2 + 3a_3 t_2^2$

Solving these equations step-by-step:

From equation 2:

$$
a_1 = v_1 - 2a_2 t_1 - 3a_3 t_1^2
$$

From equation 4:

$$
a_1 = v_2 - 2a_2 t_2 - 3a_3 t_2^2
$$

Equating these two expressions for $a_1$:

$$
v_1 - v_2 = 2a_2 (t_2 - t_1) + 3a_3 (t_2^2 - t_1^2)
$$

Solving for $a_2$:

$$
a_2 = \frac{v_1 - v_2 - 3a_3 (t_2^2 - t_1^2)}{2(t_2 - t_1)}
$$

Substitute $a_2$ into equation 1:

$$
p_1 = a_0 + v_1 t_1 - 2a_2 t_1^2 - 3a_3 t_1^3 + a_2 t_1^2 + a_3 t_1^3
$$

$$
p_1 = a_0 + v_1 t_1 - a_2 t_1^2 - 2a_3 t_1^3
$$

From equation 3:

$$
p_2 = a_0 + v_2 t_2 - a_2 t_2^2 - 2a_3 t_2^3
$$

Subtract these two equations:

$$
p_2 - p_1 = v_2 t_2 - v_1 t_1 - a_2 (t_2^2 - t_1^2) - 2a_3 (t_2^3 - t_1^3)
$$

Substitute $a_2$ back into the equation:

$$
p_2 - p_1 = v_2 t_2 - v_1 t_1 - \left(\frac{v_1 - v_2 - 3a_3 (t_2^2 - t_1^2)}{2(t_2 - t_1)}\right) (t_2^2 - t_1^2) - 2a_3 (t_2^3 - t_1^3)
$$

Simplify to solve for $a_3$:

$$
p_2 - p_1 = v_2 t_2 - v_1 t_1 - \frac{(v_1 - v_2)(t_2^2 - t_1^2)}{2(t_2 - t_1)} - \frac{3a_3 (t_2^2 - t_1^2)^2}{2(t_2 - t_1)} - 2a_3 (t_2^3 - t_1^3)
$$

$$
a_3 = \frac{2(t_2 - t_1)(p_2 - p_1) - (v_2 t_2 - v_1 t_1)(t_2 - t_1) - \frac{(v_1 - v_2)(t_2^2 - t_1^2)}{2}}{3(t_2^2 - t_1^2)^2 + 2(t_2^3 - t_1^3)(t_2 - t_1)}
$$

Now, substitute $a_3$ back to find $a_2$:

$$
a_2 = \frac{v_1 - v_2 - 3a_3 (t_2^2 - t_1^2)}{2(t_2 - t_1)}
$$

Next, solve for $a_1$:

$$
a_1 = v_1 - 2a_2 t_1 - 3a_3 t_1^2
$$

Finally, solve for $a_0$:

$$
a_0 = p_1 - a_1 t_1 - a_2 t_1^2 - a_3 t_1^3
$$
