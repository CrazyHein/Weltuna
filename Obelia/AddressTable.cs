using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMEC.PCSoftware.RemoteConsole.CrazyHein.MitsubishiControllerWorks.Tool.Obelia
{
    public enum RJ71EC92_ADDRESS_TABLE
    {
        BUF_INPUT_DATA_PTR = 0,
        BUF_OUTPUT_DATA_PTR = (24576),

        BUF_INPUT_DATA_READ_RESPONSE = (57344),
        BUF_OUTPUT_DATA_READ_REQUEST = (57345),
        BUF_INPUT_DATA_READ_REQUEST = (57346),
        BUF_OUTPUT_DATA_READ_RESPONSE = (57347),

        BUF_CONTROL_COMMAND = (65552),
        BUF_CONTROL_RESPONSE = (69648),

        BUF_NUM_OF_SLAVES = (69632),
        BUF_CFG_STATUS = (69633),
        BUF_COM_STATUS = (69634),
        BUF_MASTER_ERR_STATUS = (69635),
        BUF_CABLE_ERR_STATUS = (69636),
        BUF_MODULE_ERR_INFO = (69664),

        BUF_MASTER_ESM_STATE = (69888),
        BUF_SLAVE_ESM_STATE = (69889),

        BUF_SLAVE_ERROR_STATUS = (76288),

        BUF_EVENT_HEAD = (81920),
        BUF_EVENT_INFO = (81922),

        BUF_SDO_CONTROL_COMMAND = 90112,
        BUF_SDO_TRANSMIT_SLAVE_ADDRESS = (90113),
        BUF_SDO_TRANSMIT_INDEX = (90114),
        BUF_SDO_TRANSMIT_SUB_INDEX = (90115),
        BUF_SDO_TRANSMIT_DATA_SIZE = (90116),
        BUF_SDO_TRANSMIT_DATA_PTR = (90118),

        BUF_SDO_CONTROL_RESPONSE = (90518),
        BUF_SDO_RECEIVED_SLAVE_ADDRESS = (90519),
        BUF_SDO_RECEIVED_INDEX = (90520),
        BUF_SDO_RECEIVED_SUB_INDEX = (90521),
        BUF_SDO_RECEIVED_DATA_SIZE = (90522),
        BUF_SDO_RECEIVED_DATA_PTR = (90524),
        BUF_SDO_CONTROL_ERROR = (90924),

        BUF_MASTER_ESM_REQUEST = (98304),
        BUF_SLAVE_ESM_REQUEST = 98305,
        BUF_MASTER_ESM_RESPONSE = (99072),
        BUF_SLAVE_ESM_RESPONSE = (99073),
        BUF_MASTER_ESM_REQUEST_RET = (99840),
        BUF_SLAVE_ESM_REQUEST_RET = (99841),

    }

    public class RJ71EC92
    {
        public const int SLAVES_CAPACITY = 128;
        public const int BATCH_RW_POINTS = 960;
        public const int EVENTS_HISTORY_CAPACITY = 100;
        public const int SDO_DATA_SIZE_IN_WORD = 400;
        public const int SDO_DATA_SIZE_IN_BYTE = 800;
        public const int INPUT_DATA_SIZE_IN_WORD = 16384;
        public const int OUTPUT_DATA_SIZE_IN_WORD = 16384;

        public static IReadOnlyDictionary<uint, string> EventCodes = new Dictionary<uint, string>()
        {
            {0x00400, "信息 - ESM状态迁移" },
            {0x00401, "信息 - 以太网链接 (电缆) 连接" },
            {0x00403, "信息 - Distributed Clock初始化" },
            {0x00404, "信息 - DC SubDevice同步偏差通知|DC SubDevice的同步通知。（SubDevice已同步/不同步）" },
            {0x00406, "信息 - DCM错误状态更改|DCM同步通知。（DCM已同步/不同步）" },
            {0x00408, "信息 - SubDevice状态迁移正常完成" },
            {0x0040B, "信息 - SubDevice出现/消失" },
            {0x0040D, "信息 - 参考时钟出现/消失" },
            {0x00415, "信息 - MainDevice初始化命令: 工作计数器错误" },
            {0x00416, "信息 - SubDevice初始化命令: 工作计数器错误" },
            {0x00419, "信息 - 发送的以太网帧无响应" },
            {0x0041A, "信息 - 发送的ecat MainDevice初始化命令无响应" },
            {0x0041B, "信息 - mailbox初始化命令响应超时" },
            {0x0041D, "信息 - 以太网链接未连接" },
            {0x0041E, "信息 - 电缆冗余断开|检测到电缆冗余断开。" },
            {0x00420, "信息 - SubDevice错误状态信息|SubDevice的异常信息已通知。" },
            {0x00424, "信息 - 客户端注册已删除" },
            {0x00425, "信息 - 电缆冗余恢复|断开的电缆冗余已恢复。" },
            {0x00427, "信息 - 接收到无效的mailbox数据" },
            {0x00428, "信息 - 不支持的SubDevice (启用冗余，SubDevice不完全支持自动关闭)" },
            {0x00429, "信息 - 意外状态的SubDevice" },
            {0x0042A, "信息 - 所有SubDevice处于OP状态" },
            {0x0042C, "信息 - 检测到EEPROM校验和错误|检测到SubDevice的EEPROM校验和错误。" },
            {0x00439, "信息 - 热插拔组重新连接检测完成通知|热插拔组重新连接后，检测已完成。" },
            {0x0043A, "信息 - 热插拔组断开连接检测完成通知|热插拔组断开连接后，检测已完成。" },
            {0x0043B, "信息 - 拓扑更改完成通知|拓扑更改已完成。" },
            {0x00451, "信息 - 模块扩展参数已更新|从CPU模块读取的参数更新了智能功能模块内的模块扩展参数。" },
            {0x00452, "信息 - 重启时重新读取模块扩展参数|通信停止请求/通信恢复请求时，开始重新读取模块扩展参数。" },
            {0x00460, "信息 - 配置状态已执行|配置状态已执行。" },
            {0x00461, "信息 - 配置状态未执行|配置状态未执行。" },
            {0x00465, "信息 - <<本机>> 通信停止|通信状态已转移到通信未执行状态。" },
            {0x00466, "信息 - <<本机>> 通信开始|通信状态已转移到通信中状态。" },
            {0x00800, "信息 - <<本机>> 断线检测|检测到断线。通信状态已转移到断线中状态。" },
            {0x00801, "信息 - SubDevice通信开始失败|SubDevice通信开始失败。" },
            {0x24000, "警告 - 错误解除|通过工程工具解除错误。" },
            {0x24001, "警告 - 错误解除|通过设备YnF解除错误。" },
            {0x24002, "信息 - 事件信息清除|通过缓冲内存访问执行事件信息清除。" },
            {0x24005, "信息 - 模块扩展参数已更新|通过工程工具的写入操作更新智能功能模块内的模块扩展参数。" },
            {0x24010, "信息 - 通信停止请求命令开始|通过CPU模块缓冲内存访问开始通信停止请求。" },
            {0x24011, "信息 - 通信停止请求命令完成|通过CPU模块缓冲内存访问完成通信停止请求。" },
            {0x24015, "信息 - 通信恢复请求命令开始|通过CPU模块缓冲内存访问开始通信恢复请求。" },
            {0x24016, "信息 - 通信恢复请求命令完成|通过CPU模块缓冲内存访问完成通信恢复请求。" },
            {0x2401A, "信息 - MainDevice ESM状态更改请求完成|通过CPU模块缓冲内存访问完成MainDevice ESM状态更改请求。" },
            {0x2401B, "信息 - SubDevice ESM状态更改请求完成|通过CPU模块缓冲内存访问完成SubDevice ESM状态更改请求。" },

            {0x1800,  "错误 - 总线不匹配（模块扩展参数与连接状态不一致）" },
            {0x1803,  "错误 - MainDevice ESM状态迁移失败" },
            {0x180C,  "错误 - MailBox发送工作计数器错误|MailBox写入命令发生工作计数器错误。" },
            {0x180D,  "错误 - 初始化命令错误|SubDevice未正确响应RJ71EC92的初始化命令。" },
            {0x180E,  "错误 - SDO中止错误|初始化命令发送过程中SDO传输中断。" },
            {0x180F,  "错误 - PDI看门狗超时|PDI看门狗超时（未收到SubDevice响应）。" },
            {0x1810,  "错误 - 连接异常|MainDevice或SubDevice连接的端口错误。" },
            {0x1811,  "错误 - CPU模块停止错误|检测到CPU模块停止错误。" },
            {0x1890,  "错误 - 控制命令值异常|控制命令的指定超出范围。" },
            {0x18A0,  "错误 - 通信启动失败|在通信启动或停止处理过程中请求通信启动，导致通信启动失败。" },
            {0x18A1,  "错误 - 通信启动失败|在通信过程中请求通信启动，导致通信启动失败。" },
            {0x18A2,  "错误 - 通信启动失败|未能获取模块扩展参数，导致通信启动失败。" },
            {0x18A3,  "错误 - 通信启动失败|检测到参数设置异常，导致通信启动失败。" },
            {0x18A4,  "错误 - 通信启动失败|检测到参数或EtherCAT功能异常，导致通信启动失败。" },
            {0x18A5,  "错误 - 通信启动失败|解析模块扩展参数失败，导致通信启动失败。" },
            {0x18A6,  "错误 - 通信启动失败|总线扫描超时。" },
            {0x18A7,  "错误 - 通信启动失败|通信启动的MainDevice ESM状态迁移检测到异常。" },
            {0x18A8,  "错误 - 通信启动失败|通信启动的MainDevice ESM状态迁移检测到异常。" },
            {0x18A9,  "错误 - 通信启动失败|由于存在无法与电缆冗余同时使用的设置，通信启动失败。" },
            {0x18B0,  "错误 - 通信停止失败|在通信启动或停止处理中请求通信停止，导致通信停止失败。" },
            {0x18B1,  "错误 - 通信停止失败|在通信停止中请求通信停止，导致通信停止失败。" },
            {0x18B2,  "错误 - 通信停止失败|通信停止的MainDevice ESM状态迁移检测到异常。" },
            {0x18B3,  "错误 - 通信停止失败|通信停止的MainDevice ESM状态迁移检测到异常。" },
            {0x18B4,  "错误 - 通信停止失败|检测到EtherCAT功能异常，导致通信停止失败。" },
            {0x18C0,  "错误 - MainDevice ESM状态更改失败|在MainDevice ESM状态迁移处理中请求MainDevice ESM状态迁移，导致状态迁移失败。" },
            {0x18C1,  "错误 - MainDevice ESM状态更改失败|在通信停止中请求MainDevice ESM状态迁移，导致状态迁移失败。" },
            {0x18C2,  "错误 - MainDevice ESM状态更改失败|MainDevice的目标ESM状态指定超出范围。" },
            {0x18C3,  "错误 - MainDevice ESM状态更改失败|MainDevice ESM状态迁移检测到异常。" },
            {0x18C4,  "错误 - MainDevice ESM状态更改失败|通过缓冲内存“MainDevice ESM状态更改请求”指定的SubDevice启动ESM状态设置超出范围。" },
            {0x18E0,  "错误 - SDO通信失败|SDO控制命令指定超出范围。" },
            {0x18E1,  "错误 - SDO通信失败|在SDO通信处理中请求SDO通信，导致通信失败。" },
            {0x18E2,  "错误 - SDO通信失败|在通信停止中请求SDO通信，导致SDO通信失败。" },
            {0x18E3,  "错误 - SDO通信失败|由于SDO通信数据大小超出上限，导致SDO通信失败。" },
            {0x18E4,  "错误 - SDO通信失败|检测到SDO通信异常。" },
            {0x1900,  "错误 - MainDevice发送异常|MainDevice未能在ERTT时间内完成发送，导致发送失败。" },
            {0x1901,  "错误 - MainDevice发送异常|MainDevice未能在ERTT开始前准备好发送，导致发送失败。" },
            {0x1902,  "错误 - MainDevice发送异常|MainDevice未能在通信周期内完成处理。" },
            {0x1903,  "错误 - MainDevice发送异常|MainDevice接收的数据损坏，导致接收失败。" },
            {0x1904,  "错误 - MainDevice发送异常|MainDevice接收到超出处理能力的大量EtherCAT帧，导致部分接收失败。" },
            {0x2220,  "错误 - 参数异常|参数内容已损坏。" },
            {0x3001,  "错误 - 模块扩展参数打开失败|CPU模块的模块扩展参数已打开/同时访问的文件数量超过最大值。"},
            {0x3002,  "错误 - 模块扩展参数打开失败|RJ71EC92或CPU模块中不存在模块扩展参数。"},
            {0x3003,  "错误 - 模块扩展参数打开失败|CPU模块的模块扩展参数打开处理失败。"},
            {0x3004,  "错误 - 模块扩展参数打开失败|从GX Works3读取或写入模块扩展参数时失败。"},
            {0x3E80,  "错误 - ROM检查测试错误"},
            {0x3E81,  "错误 - 定时器测试错误"},
            {0x3E82,  "错误 - MPU测试错误"},
            {0x3E83,  "错误 - RAM测试错误"},
            {0x3E84,  "错误 - 2端口RAM测试错误"},
            {0x3E85,  "错误 - 交换端口测试错误"},
            {0x3E86,  "错误 - 闪存ROM初始化错误"},
            {0x8000,  "错误 - 模块扩展参数访问失败"},
            {0x8001,  "错误 - 模块扩展参数异常"},
            {0x8002,  "错误 - 帧异常|访问模块扩展参数的请求帧数据已损坏。"},
        };
    }
}
