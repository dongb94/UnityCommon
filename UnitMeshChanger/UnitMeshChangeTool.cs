using System.Collections.Generic;
using System.Linq;

public static class UnitMeshChangeTool
{
    /// <summary>
    /// 반드시 열거형 상수명이 모델에 등록된 교체용 오브젝트 이름의 접미사와 일치해야한다.
    /// </summary>
    public enum LamiereUnitParts
    {
        arm,
        body,
        boots,
        head,
        weapon,
    }

    public static LamiereUnitParts[] _Enumerator;
    public static Dictionary<LamiereUnitParts, string> _NameEnumearator;

    static UnitMeshChangeTool()
    {
        _Enumerator = SystemTool.GetEnumEnumerator<LamiereUnitParts>();
        _NameEnumearator = _Enumerator.ToDictionary(enumType => enumType, enumType => enumType.ToString());
    }
}