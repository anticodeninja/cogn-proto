MAX = 60

def convert1(v):
    return v / MAX

def convert2(beg, s):
    p1 = s.index('[', beg)
    p2 = s.index(',', p1)
    return s[:p1+1] + str(convert1(float(s[p1+1:p2]))) + s[p2:]

def convert3(beg, t):
    t = t.split('\n')
    for i in range(len(t)):
        if not t[i]:
            continue
        t[i] = convert2(beg, t[i])
    return '\n'.join(t)

print(convert3(0,"""
"""))
