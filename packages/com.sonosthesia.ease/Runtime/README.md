First, we have the cubic polynomial:
$p(t) = a_0 + a_1 t + a_2 t^2 + a_3 t^3 \]$

And its derivative for velocity:
$v(t) = \frac{dp(t)}{dt} = a_1 + 2a_2 t + 3a_3 t^2$

Using the boundary conditions:
$p(t_1) = p_1$
$v(t_1) = v_1$
$p(t_2) = p_2$
$v(t_2) = v_2$

We set up the following equations:

1. $p_1 = a_0 + a_1 t_1 + a_2 t_1^2 + a_3 t_1^3$
2. $v_1 = a_1 + 2a_2 t_1 + 3a_3 t_1^2$
3. $p_2 = a_0 + a_1 t_2 + a_2 t_2^2 + a_3 t_2^3$
4. $v_2 = a_1 + 2a_2 t_2 + 3a_3 t_2^2$

Solving these equations step-by-step:

From equation 2:
$a_1 = v_1 - 2a_2 t_1 - 3a_3 t_1^2$

From equation 4:
$a_1 = v_2 - 2a_2 t_2 - 3a_3 t_2^2$

Equating these two expressions for \( a_1 \):
$v_1 - v_2 = 2a_2 (t_2 - t_1) + 3a_3 (t_2^2 - t_1^2)$

Solving for \( a_2 \):
$a_2 = \frac{v_1 - v_2 - 3a_3 (t_2^2 - t_1^2)}{2(t_2 - t_1)}$

Substitute \( a_2 \) into equation 1:
$$
p_1 = a_0 + v_1 t_1 - 2a_2 t_1^2 - 3a_3 t_1^3 + a_2 t_1^2 + a_3 t_1^3
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

Simplify to solve for \( a_3 \):
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

Here are the final equations in LaTeX:

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